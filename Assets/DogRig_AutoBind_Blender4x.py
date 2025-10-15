# Blender 4.x
# 一鍵：合併目標網格 → 生成 Dog_Rig 骨架 → K-means 自動對準四肢 → 自動綁定
# 作者：GPT-5 Thinking
#
# 功能：
# - 找名稱含 TARGET_KEY 的網格（找不到就挑體積最大的 Mesh），多個則合併
# - 一定新建骨架 Dog_Rig（含 chest/pelvis 與四肢三節骨，符合 front_/hind_ 命名）
# - 以底部 Z-band + K-means(4) 偵測四隻腳中心，對齊 upper/lower/paw
# - 自動綁定：'auto' = With Automatic Weights；'group' = 依腿分組 Assign Automatic from Bones（較不會互牽）
#
# 參數：
TARGET_KEY    = "corgi_v8"
PAW_Z_BAND    = 0.12     # 取底部 12% 高度作為可能的腳點
BIND_MODE     = 'auto'   # 'auto' | 'group' | None
SEED          = 42

import bpy, random
from mathutils import Vector

# ---------- 3D 視窗覆寫 ----------
def get_view3d_area_region_window():
    for win in bpy.context.window_manager.windows:
        for area in win.screen.areas:
            if area.type == "VIEW_3D":
                for region in area.regions:
                    if region.type == "WINDOW":
                        return win, area, region
    return bpy.context.window, None, None

def set_mode(mode):
    if bpy.context.mode == mode:
        return
    win, area, region = get_view3d_area_region_window()
    with bpy.context.temp_override(window=win, area=area, region=region):
        try: bpy.ops.object.mode_set(mode=mode)
        except: pass

# ---------- Mesh 搜尋/外框 ----------
def mesh_volume(ob):
    bb = [ob.matrix_world @ Vector(c) for c in ob.bound_box]
    xs=[c.x for c in bb]; ys=[c.y for c in bb]; zs=[c.z for c in bb]
    return (max(xs)-min(xs))*(max(ys)-min(ys))*(max(zs)-min(zs))

def find_target_meshes():
    ms = [o for o in bpy.data.objects if o.type=='MESH' and o.visible_get()]
    with_key = [o for o in ms if TARGET_KEY in o.name]
    if with_key:
        return with_key
    if not ms:
        raise RuntimeError("場景內沒有任何 Mesh。")
    # 沒指定就取體積最大一個
    return [max(ms, key=mesh_volume)]

def merge_meshes(objs):
    if len(objs)==1:
        return objs[0]
    set_mode('OBJECT')
    for o in bpy.context.selected_objects: o.select_set(False)
    for o in objs: o.select_set(True)
    bpy.context.view_layer.objects.active = objs[0]
    win, area, region = get_view3d_area_region_window()
    with bpy.context.temp_override(window=win, area=area, region=region):
        bpy.ops.object.join()
    return bpy.context.view_layer.objects.active

def bbox_info(obj):
    mw = obj.matrix_world
    bb = [mw @ Vector(c) for c in obj.bound_box]
    xs=[c.x for c in bb]; ys=[c.y for c in bb]; zs=[c.z for c in bb]
    cx=(min(xs)+max(xs))/2; cy=(min(ys)+max(ys))/2; cz=(min(zs)+max(zs))/2
    hx=(max(xs)-min(xs))/2; hy=(max(ys)-min(ys))/2; hz=(max(zs)-min(zs))/2
    mins = Vector((min(xs),min(ys),min(zs)))
    maxs = Vector((max(xs),max(ys),max(zs)))
    return cx,cy,cz,hx,hy,hz,mins,maxs

def world_verts(obj):
    mw = obj.matrix_world
    return [mw @ v.co for v in obj.data.vertices]

# ---------- K-means 2D/3D ----------
def kmeans(points, k=4, iters=40, seed=SEED):
    random.seed(seed)
    centers = random.sample(points, k) if len(points)>=k else points[:]
    for _ in range(iters):
        groups = [[] for __ in range(k)]
        for p in points:
            idx = min(range(len(centers)), key=lambda i:(p-centers[i]).length_squared)
            groups[idx].append(p)
        new = []
        for g in groups:
            if g:
                s = Vector((0,0,0))
                for p in g: s += p
                new.append(s/len(g))
            else:
                new.append(random.choice(points))
        if all((new[i]-centers[i]).length < 1e-5 for i in range(len(new))):
            centers = new; break
        centers = new
    return centers

# ---------- 建骨架（一定新建 Dog_Rig） ----------
def build_armature(mesh):
    cx,cy,cz,hx,hy,hz,mins,maxs = bbox_info(mesh)
    arm_data = bpy.data.armatures.new("Dog_ArmatureData")
    arm = bpy.data.objects.new("Dog_Rig", arm_data)
    bpy.context.collection.objects.link(arm)

    set_mode('OBJECT')
    bpy.context.view_layer.objects.active = arm
    win, area, region = get_view3d_area_region_window()
    with bpy.context.temp_override(window=win, area=area, region=region):
        bpy.ops.object.mode_set(mode='EDIT')
    eb = arm.data.edit_bones

    def bone(name, head, tail, parent=None, connect=False):
        b = eb.new(name); b.head=head; b.tail=tail
        if parent: b.parent=parent; b.use_connect = connect
        return b

    # 主幹（多一個 chest / pelvis 供對準）
    root   = bone("root",   Vector((cx,cy,cz-0.22*hz)), Vector((cx,cy,cz-0.05*hz)))
    spine  = bone("spine",  root.tail, Vector((cx,cy+0.20*hy,cz+0.05*hz)), parent=root, connect=True)
    chest  = bone("chest",  spine.tail, Vector((cx,cy+0.38*hy,cz+0.18*hz)), parent=spine, connect=True)
    neck   = bone("neck",   chest.tail, Vector((cx,cy+0.55*hy,cz+0.32*hz)), parent=chest, connect=True)
    head   = bone("head",   neck.tail,  Vector((cx,cy+0.80*hy,cz+0.45*hz)), parent=neck, connect=True)
    pelvis = bone("pelvis", spine.head, Vector((cx,cy-0.20*hy,cz+0.05*hz)), parent=spine, connect=False)

    # 尾巴
    t1 = bone("tail_01", Vector((cx,cy-0.35*hy,cz+0.05*hz)), Vector((cx,cy-0.55*hy,cz+0.20*hz)), parent=pelvis)
    t2 = bone("tail_02", t1.tail, Vector((cx,cy-0.80*hy,cz+0.30*hz)), parent=t1, connect=True)

    # 四肢（命名符合 front_/hind_ upper/lower/paw）
    def leg(prefix, xmul, ymul):
        hip   = Vector((cx + xmul*0.28*hx, cy + ymul*0.32*hy, cz + 0.06*hz))
        knee  = Vector((cx + xmul*0.28*hx, cy + ymul*0.34*hy, cz - 0.06*hz))
        ankle = Vector((cx + xmul*0.28*hx, cy + ymul*0.38*hy, cz - 0.20*hz))
        paw   = Vector((cx + xmul*0.30*hx, cy + ymul*0.45*hy, cz - 0.30*hz))
        up = bone(f"{prefix}_upper", hip,   knee,  parent=(chest if 'front' in prefix else pelvis))
        lo = bone(f"{prefix}_lower", knee,  ankle, parent=up, connect=True)
        pw = bone(f"{prefix}_paw",   ankle, paw,   parent=lo, connect=True)
        return up,lo,pw

    leg("front_L", +1, +1); leg("front_R", -1, +1)
    leg("hind_L",  +1, -1); leg("hind_R",  -1, -1)

    with bpy.context.temp_override(window=win, area=area, region=region):
        bpy.ops.object.mode_set(mode='OBJECT')
    arm.show_in_front = True
    return arm

# ---------- 四肢自動對準 ----------
def auto_align_limbs(mesh, rig):
    cx,cy,cz,hx,hy,hz,mins,maxs = bbox_info(mesh)
    verts = world_verts(mesh)
    z_cut = mins.z + (maxs.z - mins.z) * PAW_Z_BAND
    bottom = [p for p in verts if p.z <= z_cut]
    if len(bottom) < 20:
        z_cut = mins.z + (maxs.z - mins.z) * max(PAW_Z_BAND*1.6, 0.18)
        bottom = [p for p in verts if p.z <= z_cut]
    if len(bottom) < 8:
        raise RuntimeError("底部頂點太少，無法自動偵測四隻腳。請調整 PAW_Z_BAND 或先合併零件。")

    centers = kmeans(bottom, k=4, iters=40)
    def label(c):
        side = "L" if c.x >= cx else "R"   # X 左右（依你的場景軸向可互換）
        part = "front" if c.y >= cy else "hind"  # Y 前後
        return f"{part}_{side}"
    clus = {}
    for c in centers:
        key = label(c)
        if key in clus:
            # 若撞名，保留較低的
            if c.z < clus[key].z: clus[key] = c
        else:
            clus[key] = c

    # 缺的用外框補位
    for key, dx, dy in [
        ("front_L", +0.26*hx, +0.40*hy),
        ("front_R", -0.26*hx, +0.40*hy),
        ("hind_L",  +0.24*hx, -0.20*hy),
        ("hind_R",  -0.24*hx, -0.20*hy),
    ]:
        if key not in clus:
            clus[key] = Vector((cx+dx, cy+dy, mins.z))

    chest = rig.data.bones.get("chest")
    pelvis= rig.data.bones.get("pelvis")
    if not (chest and pelvis):
        raise RuntimeError("缺少 chest/pelvis 骨頭。")

    chest_z  = (rig.matrix_world @ chest.head_local).z
    pelvis_z = (rig.matrix_world @ pelvis.head_local).z

    # 進 Edit 模式對齊
    bpy.context.view_layer.objects.active = rig
    set_mode('EDIT')
    def set_chain(part, side):
        pref = f"{part}_{side}"
        eb_u = rig.data.edit_bones.get(f"{pref}_upper")
        eb_l = rig.data.edit_bones.get(f"{pref}_lower")
        eb_p = rig.data.edit_bones.get(f"{pref}_paw")
        if not (eb_u and eb_l and eb_p):
            print(f"[WARN] 缺少 {pref} 的某些骨頭，跳過")
            return
        c = clus[pref]
        paw_pos = Vector((c.x, c.y, mins.z + 0.01*(maxs.z-mins.z)))
        hip_z   = chest_z if part=="front" else pelvis_z
        knee_z  = paw_pos.z*0.6 + hip_z*0.4
        hip_x   = cx*0.7 + c.x*0.3
        hip_y   = cy*0.7 + c.y*0.3

        eb_u.head = Vector((hip_x, hip_y, hip_z))
        eb_u.tail = Vector((hip_x, hip_y, knee_z))
        eb_l.head = eb_u.tail.copy()
        eb_l.tail = Vector((c.x, c.y, (knee_z + paw_pos.z)*0.5))
        eb_p.head = eb_l.tail.copy()
        eb_p.tail = paw_pos

    for part in ("front","hind"):
        for side in ("L","R"):
            set_chain(part, side)

    set_mode('OBJECT')
    print("[OK] 四肢已自動對準。")

# ---------- 綁定 ----------
def apply_transform(obj):
    set_mode('OBJECT')
    bpy.context.view_layer.objects.active = obj
    win, area, region = get_view3d_area_region_window()
    with bpy.context.temp_override(window=win, area=area, region=region, active_object=obj, object=obj):
        try: bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)
        except: pass

def bind_auto(mesh, arm):
    # 清舊 Armature modifier
    for m in list(mesh.modifiers):
        if m.type=='ARMATURE':
            mesh.modifiers.remove(m)
    mesh.modifiers.new("Armature",'ARMATURE').object = arm
    # 親子：With Automatic Weights
    for o in bpy.context.selected_objects: o.select_set(False)
    mesh.select_set(True); arm.select_set(True)
    bpy.context.view_layer.objects.active = arm
    win, area, region = get_view3d_area_region_window()
    with bpy.context.temp_override(window=win, area=area, region=region,
                                   active_object=arm, object=arm,
                                   selected_objects=[mesh, arm],
                                   selected_editable_objects=[mesh, arm]):
        bpy.ops.object.parent_set(type='ARMATURE_AUTO')

def bind_grouped(mesh, arm):
    """分腿分組綁：可避免前後腿互牽"""
    # 先建立空群組親子
    for m in list(mesh.modifiers):
        if m.type=='ARMATURE':
            mesh.modifiers.remove(m)
    mesh.modifiers.new("Armature",'ARMATURE').object = arm
    for o in bpy.context.selected_objects: o.select_set(False)
    mesh.select_set(True); arm.select_set(True)
    bpy.context.view_layer.objects.active = arm
    win, area, region = get_view3d_area_region_window()
    with bpy.context.temp_override(window=win, area=area, region=region,
                                   active_object=arm, object=arm,
                                   selected_objects=[mesh, arm],
                                   selected_editable_objects=[mesh, arm]):
        bpy.ops.object.parent_set(type='ARMATURE_NAME')  # Empty Groups

    # 逐組 Assign Automatic From Bones
    groups = {
        "front_L": ["front_upper_L","front_lower_L","front_paw_L"],
        "front_R": ["front_upper_R","front_lower_R","front_paw_R"],
        "hind_L":  ["hind_upper_L","hind_lower_L","hind_paw_L"],
        "hind_R":  ["hind_upper_R","hind_lower_R","hind_paw_R"],
        "trunk":   ["spine","chest","neck","head","pelvis","tail_01","tail_02"],
    }
    for key in ["front_L","front_R","hind_L","hind_R","trunk"]:
        # 選骨
        set_mode('POSE')
        for pb in arm.pose.bones: pb.bone.select = False
        for bn in groups[key]:
            pb = arm.pose.bones.get(bn)
            if pb: pb.bone.select = True
        # 到網格 Weight Paint，Assign Automatic from Bones
        set_mode('OBJECT')
        mesh.select_set(True); bpy.context.view_layer.objects.active = mesh
        win, area, region = get_view3d_area_region_window()
        with bpy.context.temp_override(window=win, area=area, region=region, active_object=mesh, object=mesh):
            bpy.ops.object.mode_set(mode='WEIGHT_PAINT')
            bpy.ops.paint.weight_from_bones(type='AUTOMATIC')
            bpy.ops.object.mode_set(mode='OBJECT')

    # 清理權重
    mesh.select_set(True); bpy.context.view_layer.objects.active = mesh
    try: bpy.ops.object.vertex_group_limit_total(limit=3)
    except: pass
    try: bpy.ops.object.vertex_group_clean(group_select_mode='ALL', limit=0.02, keep_single=True)
    except: pass
    try: bpy.ops.object.vertex_group_normalize_all(lock_active=False)
    except: pass

# ================= 主流程 =================
# 1) 找/合併網格
targets = find_target_meshes()
mesh = merge_meshes(targets)

# 2) 新建骨架
arm = build_armature(mesh)

# 3) 套用變換，避免權重比例異常
apply_transform(mesh); apply_transform(arm)

# 4) 自動對準四肢
auto_align_limbs(mesh, arm)

# 5) 綁定
if BIND_MODE == 'auto':
    bind_auto(mesh, arm)
elif BIND_MODE == 'group':
    bind_grouped(mesh, arm)

# 6) 切 Pose Mode
set_mode('POSE')
print("✅ 完成：骨架建立 + 四肢對準" + (f" + 綁定模式={BIND_MODE}" if BIND_MODE else "（未綁定）")) 
