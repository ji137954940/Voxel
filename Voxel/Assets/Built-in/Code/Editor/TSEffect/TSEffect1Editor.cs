using UnityEngine;
using System.Collections;
using UnityEditor;
using ZLib;
using System.Collections.Generic;
using ZEffect;

[CustomEditor(typeof(TSEffect1))]
public class TSEffect1Editor : Editor
{
    //目标脚本对象
    TSEffect1 effect;

    //普通数据设置
    bool General = true;
    //飞行数据设置
    bool FlySet = true;
    //贝塞尔曲线控制点数据显示
    List<bool> ShowBeizerPointList = new List<bool>();


    //脚本启用
    void OnEnable()
    {
        // 获得目标脚本
        effect = target as TSEffect1;
    }

    /// <summary>
    /// 输出数据
    /// </summary>
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        GUILayout.Space(10);

        #region 显示普通属性数据

        General = HTGuiTools.BeginFoldOut("General Properties", General);

        if (General)
        {
            HTGuiTools.BeginGroup();
            {
                GUILayout.Space(10);

                // 是否为跟随
                effect.IsFollow = EditorGUILayout.Toggle("Is Follow", effect.IsFollow);
                //EditorGUI.BeginDisabledGroup(effect.IsFollow == false);
                
                effect.IgnoreRotation = EditorGUILayout.Toggle("Ignore Rotation", effect.IgnoreRotation);
                //EditorGUI.EndDisabledGroup();

                //是否自动销毁
                effect.IsAutoDestory = EditorGUILayout.Toggle("Is Auto Destory", effect.IsAutoDestory);
                if (effect.IsAutoDestory)
                {
                    //自动销毁时间
                    effect.AutoDestoryTime = EditorGUILayout.FloatField("Auto Destory Time", effect.AutoDestoryTime);
                }

                //是否跟随技能自动销毁
                effect.IsAutoDestoryFollowSkill = EditorGUILayout.Toggle("Is Auto Destory Follow Skill", effect.IsAutoDestoryFollowSkill);

                //是否延迟销毁
                effect.DelayDestoryTime = EditorGUILayout.FloatField("Delay Destory Time", effect.DelayDestoryTime);
                if (effect.DelayDestoryTime != 0)
                {
                    int count = EditorGUILayout.IntField("ImmediateDestoryObjects Count", effect.ImmediateDestoryObjects.Count);
                    if(effect.ImmediateDestoryObjects.Count != count)
                    {
                        effect.ImmediateDestoryObjects.Clear();
                    }
                    //自动销毁时间
                    for(int i = 0; i < count; ++i)
                    {
                        if(effect.ImmediateDestoryObjects.Count <= i)
                        {
                            effect.ImmediateDestoryObjects.Add(null);
                        }
                        effect.ImmediateDestoryObjects[i] = (GameObject)EditorGUILayout.ObjectField("ImmediateDestoryObject " + i.ToString(), effect.ImmediateDestoryObjects[i], typeof(GameObject), true);
                    }
                }

                //是否在目标点释放
                effect.IsOnTargetPos = EditorGUILayout.Toggle("Is On TargetPos", effect.IsOnTargetPos);
                if (effect.IsOnTargetPos)
                    effect.IsFly = false;

                //设置模型缩放形式
                var est = EditorGUILayout.EnumPopup("Effect Scale Type", effect.scale_type);
                effect.scale_type = (TSEffect1.EffectScaleTypeEnum)est;

                //开始挂点位置与结束挂点位置
                var smp = EditorGUILayout.EnumPopup("Start Mount Point", effect.startMountPoint);
                effect.startMountPoint = (MountPoint.MountPointEnum)smp;

                var emp = EditorGUILayout.EnumPopup("End Mount Point", effect.endMountPoint);
                effect.endMountPoint = (MountPoint.MountPointEnum)emp;

                //关联资源id
                effect.RelationEffectId = EditorGUILayout.IntField("Relation Effect Id", effect.RelationEffectId);

                //源对象与目标对象
                effect.Src = (GameObject)EditorGUILayout.ObjectField("Src", effect.Src, typeof(GameObject), true);

                effect.Target = (GameObject)EditorGUILayout.ObjectField("Target", effect.Target, typeof(GameObject), true);

                //延迟震动时间
                effect.DelayShakeTime = EditorGUILayout.FloatField("DelayS hake Time", effect.DelayShakeTime);
                //是否是碰撞震屏
                effect.IsCollisionShake = EditorGUILayout.Toggle("Is Collision Shake", effect.IsCollisionShake);
                //震动参数--和动作帧参数一样
                effect.ShakeId = EditorGUILayout.IntField("Shake Id", effect.ShakeId);

                ShowPoints3D("Target Point", effect.targetPos);

                //拖尾对象显示
                effect.trail_arr = ShowEffectTrail(effect.trail_arr);

                if (effect.EffectHeroOnlyList == null)
                    effect.EffectHeroOnlyList = new List<GameObject>();
                ShowEffectHeroOnly(effect.EffectHeroOnlyList);

                //特效动画文件信息
                effect.anim_arr = ShowEffectAnim(effect.anim_arr);

                //特效Animation文件信息
                effect.animation_arr = ShowEffectAnimation(effect.animation_arr);

                //特效粒子文件信息
                effect.particle_arr = ShowEffectParticleSystem(effect.particle_arr);

                //模型对象文件信息
                effect.ren_arr = ShowEffectParticleSystem(effect.ren_arr);

                //模型缩放数据显示
                if (effect.scale_type != TSEffect1.EffectScaleTypeEnum.NONE)
                {
                    if (effect.scale_tran_list == null)
                        effect.scale_tran_list = new List<Transform>();
                    ShowEffectScale(effect.scale_tran_list);
                }

                //effect.EffectTrail = (GameObject)EditorGUILayout.ObjectField("Effect Trail", effect.EffectTrail, typeof(GameObject), true);
            }
            HTGuiTools.EndGroup();
        }

        #endregion

        #region 飞行数据

        //FlySet = EditorGUILayout.Foldout(FlySet, "Fly Set");
        FlySet = HTGuiTools.BeginFoldOut("Fly Properties", FlySet);
        if (FlySet)
        {
            HTGuiTools.BeginGroup();
            {
                GUILayout.Space(10);
                effect.ignoreColliderBox = EditorGUILayout.Toggle("Ignore Collider Box", effect.ignoreColliderBox);
                // 是否为飞行
                effect.IsFly = EditorGUILayout.Toggle("Is Fly", effect.IsFly);
                if (effect.IsFly)
                {
                    effect.IsOnTargetPos = false;

                    //飞行轨迹类型
                    var ft = EditorGUILayout.EnumPopup("Fly Type", effect.FlyType);
                    effect.FlyType = (TSEffect1.FlyEnum)ft;


                    switch (effect.FlyType)
                    {
                        case TSEffect1.FlyEnum.LINE:
                            //飞行速度
                            effect.FlySpeed = EditorGUILayout.FloatField("Fly Speed", effect.FlySpeed);
                            //最大碰撞距离
                            effect.MaxCollideDis = EditorGUILayout.FloatField("Max Collide Dis", effect.MaxCollideDis);

                            if (effect.MTList == null)
                                effect.MTList = new List<TSEffect1.MoveTranInfo>();

                            if (effect.MTList.Count > 0)
                            {
                                //显示限制角度大小
                                effect.StartAngle = EditorGUILayout.FloatField("Limit Angle", effect.StartAngle);
                                //显示最大移动距离
                                effect.LimitDis = EditorGUILayout.FloatField("Limit Dis", effect.LimitDis);
                                //是否只检测距离
                                effect.IsOnlyCheckDis = EditorGUILayout.Toggle("Is Only Check Dis", effect.IsOnlyCheckDis);
                            }

                            effect.BirthTargetPointRandomOffset().birthOffsetRect = ShowRect(" 初始点位置偏移范围 ", effect.BirthTargetPointRandomOffset().birthOffsetRect);

                            //effect.BirthTargetPointRandomOffset().targetOffsetRect = ShowRect(" 目标点位置偏移范围 ", effect.BirthTargetPointRandomOffset().targetOffsetRect);

                            //显示移动对象数据
                            ShowMoveTranInfo(false);

                            break;
                        case TSEffect1.FlyEnum.CURVE:
                            //飞行速度
                            effect.FlySpeed = EditorGUILayout.FloatField("Fly Speed", effect.FlySpeed);
                            //最大碰撞距离
                            effect.MaxCollideDis = EditorGUILayout.FloatField("Max Collide Dis", effect.MaxCollideDis);
                            //显示限制角度大小
                            effect.StartAngle = EditorGUILayout.FloatField("Limit Angle", effect.StartAngle);
                            //显示限制角度大小
                            effect.LimitAngleClamp = EditorGUILayout.FloatField("Limit Angle Clamp", effect.LimitAngleClamp);
                            break;
                        case TSEffect1.FlyEnum.FIXHEITHTCURVE:
                            //飞行速度
                            effect.FlySpeed = EditorGUILayout.FloatField("Fly Speed", effect.FlySpeed);
                            //最大碰撞距离
                            effect.MaxCollideDis = EditorGUILayout.FloatField("Max Collide Dis", effect.MaxCollideDis);
                            //固定高度
                            effect.FixHeightCurve_Height = EditorGUILayout.FloatField("Fixed Height", effect.FixHeightCurve_Height);
                            break;
                        case TSEffect1.FlyEnum.BEZIER:

                            //飞行速度
                            effect.FlySpeed = EditorGUILayout.FloatField("Fly Speed", effect.FlySpeed);
                            //最大碰撞距离
                            effect.MaxCollideDis = EditorGUILayout.FloatField("Max Collide Dis", effect.MaxCollideDis);
                            //是否只检测距离
                            effect.IsOnlyCheckDis = EditorGUILayout.Toggle("Is Only Check Dis", effect.IsOnlyCheckDis);
                            //贝塞尔曲线标准距离
                            effect.BezierStandardDistance = EditorGUILayout.FloatField("Bezier Standard Distance", effect.BezierStandardDistance);
                            //显示贝塞尔曲线的最小距离，如果小于此距离，那么就直接显示直线
                            effect.BezierMinDis = EditorGUILayout.FloatField("Show Bezier Min Dis", effect.BezierMinDis);
                            if (effect.BezierMinDis > 0)
                            {
                                //显示限制角度大小
                                effect.StartAngle = EditorGUILayout.FloatField("Limit Angle", effect.StartAngle);
                                //显示最大移动距离
                                effect.LimitDis = EditorGUILayout.FloatField("Limit Dis", effect.LimitDis);
                            }

                            effect.BezierRotateTimeStamp = EditorGUILayout.FloatField("Rotate Time offfset", effect.BezierRotateTimeStamp);

                            if (effect.MTList == null)
                                effect.MTList = new List<TSEffect1.MoveTranInfo>();

                            if (effect.MTList.Count == 0)
                            {
                                if (effect.PointsList == null)
                                    effect.PointsList = new List<TSEffect1.PointRangeInfo>();

                                //显示当前设置的点的集合数据
                                //ShowPoints3D(effect.PointsList);
                                ShowPoints3DRange(effect.PointsList);
                            }

                            //显示移动对象数据
                            ShowMoveTranInfo(true);

                            //显示贝塞尔曲线轨迹
                            if (GUILayout.Button("Show Bezier Path"))
                            {
                                OnShowBezierPath();
                            }
                            break;
                        case TSEffect1.FlyEnum.EJECTION:
                            //飞行速度
                            effect.FlySpeed = EditorGUILayout.FloatField("Fly Speed", effect.FlySpeed);
                            //最大碰撞距离
                            effect.MaxCollideDis = EditorGUILayout.FloatField("Max Collide Dis", effect.MaxCollideDis);
                            //显示移动对象数据
                            ShowMoveTranInfo(false);
                            break;
                        case TSEffect1.FlyEnum.RAYS:
                            //飞行速度
                            effect.FlySpeed = EditorGUILayout.FloatField("Fly Speed", effect.FlySpeed);
                            //最大碰撞距离
                            effect.MaxCollideDis = EditorGUILayout.FloatField("Max Collide Dis", effect.MaxCollideDis);
                            ////显示移动对象数据
                            //ShowMoveTranInfo(false);
                            //显示射线数据信息
                            ShowRaysInfo();
                            break;
                        case TSEffect1.FlyEnum.LIGHTNING_CHAIN:
                            //飞行速度
                            effect.FlySpeed = EditorGUILayout.FloatField("Fly Speed", effect.FlySpeed);
                            //最大碰撞距离
                            effect.MaxCollideDis = EditorGUILayout.FloatField("Max Collide Dis", effect.MaxCollideDis);

                            //显示移动对象数据
                            ShowMoveTranInfo(false);

                            //显示射线数据信息
                            ShowRaysInfo();
                            break;
                        case TSEffect1.FlyEnum.Beam:
                            //mesh texture的偏移参数
                            effect.fBeamTextureLengthScale = EditorGUILayout.FloatField("Beam Texture Length Scale", effect.fBeamTextureLengthScale);
                            //mesh texture的偏移速度
                            effect.fBeamTextureScrollSpeed = EditorGUILayout.FloatField("Beam Texture Scroll Speed", effect.fBeamTextureScrollSpeed);

                            //绘制中间连线 预设设置
                            DrawBeamLineRendererList();
                            
                            //绘制开始点特效 预设设置
                            effect.BeamStartPrefab = (GameObject)EditorGUILayout.ObjectField("Beam Start", effect.BeamStartPrefab, typeof(GameObject), true);
                            //effect.BeamEndPrefab = (GameObject)EditorGUILayout.ObjectField("Beam End", effect.BeamEndPrefab, typeof(GameObject), true);
                            break;
                        case TSEffect1.FlyEnum.CIRCLE:
                            //飞行速度
                            effect.FlySpeed = EditorGUILayout.FloatField("Fly Speed", effect.FlySpeed);
                            //旋转半径
                            effect.CircleRadius = EditorGUILayout.FloatField("Circle Radius", effect.CircleRadius);
                            //旋转特效缩放大小
                            effect.CircleScale = EditorGUILayout.FloatField("Circle Scale", effect.CircleScale);
                            //旋转特效缩放大小最大值
                            effect.MaxCircleScale = EditorGUILayout.FloatField("Max Circle Scale", effect.MaxCircleScale);
                            //旋转特效缩放大小叠加增加量
                            effect.CircleOverlayScale = EditorGUILayout.FloatField("Circle OverlayScale", effect.CircleOverlayScale);
                            //旋转特效上下偏移
                            effect.CircleOffset = EditorGUILayout.FloatField("Circle Offset", effect.CircleOffset);
                            //旋转特效上下偏移的速度
                            effect.CircleOffsetSpeed = EditorGUILayout.FloatField("Circle Offset Speed", effect.CircleOffsetSpeed);
                            break;
                    }

                }
            }
            HTGuiTools.EndGroup();
        }


        #endregion

        if (GUILayout.Button("Play"))
        {
            if (!effect.enabled)
                effect.enabled = true;
            effect.OnInit();
        }

        if (GUILayout.Button("Update All Save Info"))
        {
            UpdateAllSaveInfo();
        }
    }

  
    bool toggle = true;
    /// <summary>
    /// 激光lineRenderer列表绘制
    /// </summary>
    private void DrawBeamLineRendererList()
    {
        HTGuiTools.BeginGroup(4);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("LineRenderers : " + effect.GoBeamLineRendererPrefab.Count);
        if (GUILayout.Button(" + "))
        {
            effect.GoBeamLineRendererPrefab.Add(null);
        }
        EditorGUILayout.EndHorizontal();

        for (int i = 0; i < effect.GoBeamLineRendererPrefab.Count; i++)
        {
            EditorGUILayout.BeginHorizontal("Box");
            effect.GoBeamLineRendererPrefab[i] = (GameObject)EditorGUILayout.ObjectField("Beam LineRenderer " + i, effect.GoBeamLineRendererPrefab[i], typeof(GameObject), true);
            if (GUILayout.Button("-"))
            {
                effect.GoBeamLineRendererPrefab.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        HTGuiTools.EndGroup(true);
    }

    #region 显示点数据

    static GUILayoutOption width30 = GUILayout.Width(30);
    static GUILayoutOption width40 = GUILayout.Width(40);
    static GUILayoutOption width60 = GUILayout.Width(60);
    static GUILayoutOption height19 = GUILayout.Height(19);

    ///// <summary>
    ///// 显示存储点数据列表信息
    ///// </summary>
    //void ShowPoints3D()
    //{
    //    EditorGUILayout.BeginVertical("Box");
    //    EditorGUILayout.BeginHorizontal();
    //    GUILayout.Label("Number of points: " + effect.PointsList.Count);
    //    GUILayout.FlexibleSpace();
    //    GUI.enabled = CanAddPoint3D();
    //    if (GUILayout.Button("+", width40))
    //    {
    //        AddPoint3D(effect.PointsList.Count == 0 ? Vector3.zero : effect.PointsList[effect.PointsList.Count - 1]);
    //    }
    //    GUI.enabled = CanRemovePoint3D();
    //    GUILayout.Space(15);
    //    if (GUILayout.Button("-", width40))
    //    {
    //        RemovePoint3D(effect.PointsList.Count - 1);
    //    }
    //    GUI.enabled = true;
    //    EditorGUILayout.EndHorizontal();
    //    GUILayout.Space(5);

    //    Vector3 point;
    //    for (int i = 0; i < effect.PointsList.Count; i++)
    //    {
    //        EditorGUILayout.BeginHorizontal();
    //        GUILayout.Label("    " + i, width60, height19);
    //        GUILayout.Label("X");
    //        point.x = EditorGUILayout.FloatField(effect.PointsList[i].x, width60);
    //        GUILayout.Label(" Y");
    //        point.y = EditorGUILayout.FloatField(effect.PointsList[i].y, width60);
    //        GUILayout.Label(" Z");
    //        point.z = EditorGUILayout.FloatField(effect.PointsList[i].z, width60);

    //        if (effect.PointsList[i].x != point.x || effect.PointsList[i].y != point.y || effect.PointsList[i].z != point.z)
    //            effect.PointsList[i] = point;

    //        GUI.enabled = CanAddPoint3D();
    //        if (GUILayout.Button("+", EditorStyles.miniButton, width40))
    //        {
    //            InsertPoint3D(i, effect.PointsList[i]);
    //        }
    //        GUI.enabled = CanRemovePoint3D();
    //        if (GUILayout.Button("-", EditorStyles.miniButton, width40))
    //        {
    //            RemovePoint3D(i);
    //        }
    //        GUI.enabled = true;
    //        GUILayout.FlexibleSpace();

    //        EditorGUILayout.EndHorizontal();
    //    }
    //    EditorGUILayout.EndVertical();
    //}

    /// <summary>
    /// 显示存储点数据列表信息
    /// </summary>
    void ShowPoints3D(string label_text, Vector3 point)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(label_text);
        GUILayout.FlexibleSpace();
        GUILayout.Space(15);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("X");
            point.x = EditorGUILayout.FloatField(point.x, width60);
            GUILayout.Label(" Y");
            point.y = EditorGUILayout.FloatField(point.y, width60);
            GUILayout.Label(" Z");
            point.z = EditorGUILayout.FloatField(point.z, width60);
            GUI.enabled = true;
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 显示存储点数据列表信息
    /// </summary>
    void ShowPoints3D(List<Vector3> list)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Number of Points: " + list.Count);
        GUILayout.FlexibleSpace();
        GUI.enabled = CanAddPoint3D(list);
        if (GUILayout.Button("+", width40))
        {
            AddPoint3D(list, list.Count == 0 ? Vector3.zero : list[list.Count - 1]);
        }
        GUI.enabled = CanRemovePoint3D(list);
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            RemovePoint3D(list, list.Count - 1);
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        Vector3 point;
        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("    " + i, width60, height19);
            GUILayout.Label("X");
            point.x = EditorGUILayout.FloatField(list[i].x, width60);
            GUILayout.Label(" Y");
            point.y = EditorGUILayout.FloatField(list[i].y, width60);
            GUILayout.Label(" Z");
            point.z = EditorGUILayout.FloatField(list[i].z, width60);

            if (list[i].x != point.x || list[i].y != point.y || list[i].z != point.z)
                list[i] = point;

            GUI.enabled = CanAddPoint3D(list);
            if (GUILayout.Button("+", EditorStyles.miniButton, width40))
            {
                InsertPoint3D(list, i, list[i]);
            }
            GUI.enabled = CanRemovePoint3D(list);
            if (GUILayout.Button("-", EditorStyles.miniButton, width40))
            {
                RemovePoint3D(list, i);
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 显示存储点数据列表信息
    /// </summary>
    void ShowPoints3DRange(List<TSEffect1.PointRangeInfo> list)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Number of Points: " + list.Count);
        GUILayout.FlexibleSpace();
        GUI.enabled = (list.Count < 10 && effect.IsFly && effect.FlyType == TSEffect1.FlyEnum.BEZIER);
        if (GUILayout.Button("+", width40))
        {
            //AddPoint3D(list, list.Count == 0 ? Vector3.zero : list[list.Count - 1]);
            list.Add(new TSEffect1.PointRangeInfo());
        }
        GUI.enabled = list.Count > 0;
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            //RemovePoint3D(list, list.Count - 1);
            list.RemoveAt(list.Count - 1);
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        Vector3 minPoint;
        Vector3 maxPoint;
        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("    " + i, width60, height19);
            GUILayout.Label("X");
            minPoint.x = EditorGUILayout.FloatField(list[i].min.x, width30);
            maxPoint.x = EditorGUILayout.FloatField(list[i].max.x, width30);

            GUILayout.Label(" Y");
            minPoint.y = EditorGUILayout.FloatField(list[i].min.y, width30);
            maxPoint.y = EditorGUILayout.FloatField(list[i].max.y, width30);

            GUILayout.Label(" Z");
            minPoint.z = EditorGUILayout.FloatField(list[i].min.z, width30);
            maxPoint.z = EditorGUILayout.FloatField(list[i].max.z, width30);

            if (list[i].min.x != minPoint.x || list[i].min.y != minPoint.y || list[i].min.z != minPoint.z)
                list[i].min = minPoint;

            if (list[i].max.x != maxPoint.x || list[i].max.y != maxPoint.y || list[i].max.z != maxPoint.z)
                list[i].max = maxPoint;

            GUI.enabled = (list.Count < 10 && effect.IsFly && effect.FlyType == TSEffect1.FlyEnum.BEZIER);
            if (GUILayout.Button("+", EditorStyles.miniButton, width40))
            {
                //InsertPoint3D(list, i, list[i]);
                list.Insert(i, list[i]);
            }
            GUI.enabled = list.Count > 0;
            if (GUILayout.Button("-", EditorStyles.miniButton, width40))
            {
                //RemovePoint3D(list, i);
                list.RemoveAt(i);
            }
            GUI.enabled = true;
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 检测是否可以添加点
    /// </summary>
    /// <returns></returns>
    bool CanAddPoint3D(List<Vector3> list)
    {
        return (list.Count < 10 && effect.IsFly && effect.FlyType == TSEffect1.FlyEnum.BEZIER);
    }

    /// <summary>
    /// 是否可以移除点
    /// </summary>
    /// <returns></returns>
    bool CanRemovePoint3D(List<Vector3> list)
    {
        return (list.Count > 0);
    }

    /// <summary>
    /// 添加新的点
    /// </summary>
    /// <param name="v"></param>
    void AddPoint3D(List<Vector3> list, Vector3 v)
    {
        list.Add(v);
    }

    /// <summary>
    /// 从中间插入新的点
    /// </summary>
    /// <param name="i"></param>
    /// <param name="v"></param>
    void InsertPoint3D(List<Vector3> list, int i, Vector3 v)
    {
        if (effect.FlyType == TSEffect1.FlyEnum.BEZIER && (i & 1) != 0)
            i--;

        list.Insert(i, v);
    }

    /// <summary>
    /// 移除一个点
    /// </summary>
    /// <param name="i"></param>
    void RemovePoint3D(List<Vector3> list, int i)
    {
        if (effect.FlyType == TSEffect1.FlyEnum.BEZIER && (i & 1) != 0)
            i--;

        list.RemoveAt(i);
    }

    #endregion

    #region 显示移动对象数据

    ///// <summary>
    ///// 显示移动对象列表数据
    ///// </summary>
    //void ShowMoveTran()
    //{
    //    EditorGUILayout.BeginVertical("Box");
    //    EditorGUILayout.BeginHorizontal();
    //    GUILayout.Label("Number of Objs: " + effect.MoveTransList.Count);
    //    GUILayout.FlexibleSpace();
    //    if (GUILayout.Button("+", width40))
    //    {
    //        AddMoveTran();
    //    }
    //    GUI.enabled = CanRemoveMoveTran();
    //    GUILayout.Space(15);
    //    if (GUILayout.Button("-", width40))
    //    {
    //        RemoveMoveTran(effect.MoveTransList.Count - 1);
    //    }
    //    GUI.enabled = true;
    //    EditorGUILayout.EndHorizontal();
    //    GUILayout.Space(5);

    //    for (int i = 0; i < effect.MoveTransList.Count; i++)
    //    {
    //        EditorGUILayout.BeginHorizontal();
    //        GUILayout.Label("    " + i, width60, height19);

    //        effect.MoveTransList[i] = (Transform)EditorGUILayout.ObjectField("Move Tran", effect.MoveTransList[i], typeof(Transform), true);

    //        if (GUILayout.Button("+", EditorStyles.miniButton, width40))
    //        {
    //            InsertMoveTran(i);
    //        }
    //        GUI.enabled = CanRemoveMoveTran();
    //        if (GUILayout.Button("-", EditorStyles.miniButton, width40))
    //        {
    //            RemoveMoveTran(i);
    //        }
    //        GUI.enabled = true;
    //        //GUILayout.FlexibleSpace();

    //        EditorGUILayout.EndHorizontal();
    //    }
    //    EditorGUILayout.EndVertical();
    //}

    ///// <summary>
    ///// 显示移动对象列表数据
    ///// </summary>
    //void ShowMoveTran(bool isBezier)
    //{
    //    EditorGUILayout.BeginVertical("Box");
    //    EditorGUILayout.BeginHorizontal();
    //    GUILayout.Label("Number of Objs: " + effect.MoveTransList.Count);
    //    GUILayout.FlexibleSpace();
    //    if (GUILayout.Button("+", width40))
    //    {
    //        AddMoveTran();
    //    }
    //    GUI.enabled = CanRemoveMoveTran();
    //    GUILayout.Space(15);
    //    if (GUILayout.Button("-", width40))
    //    {
    //        RemoveMoveTran(effect.MoveTransList.Count - 1);
    //    }
    //    GUI.enabled = true;
    //    EditorGUILayout.EndHorizontal();
    //    GUILayout.Space(5);

    //    for (int i = 0; i < effect.MoveTransList.Count; i++)
    //    {

    //        EditorGUILayout.BeginVertical("Box");

    //        EditorGUILayout.BeginHorizontal();
    //        GUILayout.Label("    " + i, width60, height19);

    //        effect.MoveTransList[i] = (Transform)EditorGUILayout.ObjectField("Move Tran", effect.MoveTransList[i], typeof(Transform), true);

    //        if (GUILayout.Button("+", EditorStyles.miniButton, width40))
    //        {
    //            InsertMoveTran(i);
    //        }
    //        GUI.enabled = CanRemoveMoveTran();
    //        if (GUILayout.Button("-", EditorStyles.miniButton, width40))
    //        {
    //            RemoveMoveTran(i);
    //        }
    //        GUI.enabled = true;
    //        //GUILayout.FlexibleSpace();

    //        EditorGUILayout.EndHorizontal();

    //        //显示控制点数据信息
    //        ShowPoints3D(effect.PList[i]);

    //        EditorGUILayout.EndVertical();
    //    }
    //    EditorGUILayout.EndVertical();
    //}


    /// <summary>
    /// 显示移动对象列表数据
    /// </summary>
    void ShowMoveTranInfo(bool showPoints)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();

        string showName = null;
        if (effect.FlyType == TSEffect1.FlyEnum.EJECTION
            || effect.FlyType == TSEffect1.FlyEnum.LIGHTNING_CHAIN)
        {
            GUILayout.Label("Number of TargetTranObjs: " + effect.MTList.Count);
            showName = "Target Tran";
        }
        else
        {
            GUILayout.Label("Number of MoveTranObjs: " + effect.MTList.Count);
            showName = "Move Tran";
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", width40))
        {
            AddMoveTranInfo();
        }
        GUI.enabled = CanRemoveMoveTranInfo();
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            RemoveMoveTranInfo(effect.MTList.Count - 1);
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        bool isRemove;
        for (int i = 0; i < effect.MTList.Count; i++)
        {
            isRemove = false;
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("    " + i, width60, height19);

            effect.MTList[i].tran = (Transform)EditorGUILayout.ObjectField(showName, effect.MTList[i].tran, typeof(Transform), true);

            if (GUILayout.Button("+", EditorStyles.miniButton, width40))
            {
                InsertMoveTranInfo(i);
            }
            GUI.enabled = CanRemoveMoveTranInfo();
            if (GUILayout.Button("-", EditorStyles.miniButton, width40))
            {
                RemoveMoveTranInfo(i);
                isRemove = true;
            }
            GUI.enabled = true;
            //GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            if (showPoints && !isRemove)
            {
                //显示控制点数据信息
                ShowPoints3D(effect.MTList[i].points);
            }

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 是否可以移除移动对象数据
    /// </summary>
    /// <returns></returns>
    bool CanRemoveRayInfo()
    {
        return effect.arcList != null && effect.arcList.Count > 0;
    }

    /// <summary>
    /// 添加移动对象数据
    /// </summary>
    void AddRayInfo()
    {
        effect.arcList.Add(null);
        if (effect.arcList.Count == 1)
        {
            //如果是第一个新添加的数据
            //那么设置默认值数据
            ArcReactor_Arc arc = effect.GetComponent<ArcReactor_Arc>();
            effect.arcList[0] = arc;
        }
    }

    /// <summary>
    /// 插入移动对象数据
    /// </summary>
    /// <param name="index"></param>
    void InsertRayInfo(int index)
    {
        effect.arcList.Insert(index, null);
    }

    /// <summary>
    /// 移除移动对象数据
    /// </summary>
    /// <param name="index"></param>
    void RemoveRayInfo(int index)
    {
        effect.arcList.RemoveAt(index);
    }

    #endregion

    #region 显示 Rect 区域绘制

    /// <summary>
    /// 显示 rect 区域数据
    /// </summary>
    Rect ShowRect(string label_text, Rect rect)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(label_text);
        GUILayout.FlexibleSpace();
        GUILayout.Space(15);
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("MinX ");
        rect.xMin = EditorGUILayout.FloatField(rect.xMin, width60);
        GUILayout.Label("MinY ");
        rect.yMin = EditorGUILayout.FloatField(rect.yMin, width60);
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("MaxX");
        rect.xMax = EditorGUILayout.FloatField(rect.xMax, width60);
        GUILayout.Label("MaxY");
        rect.yMax = EditorGUILayout.FloatField(rect.yMax, width60);
        GUI.enabled = true;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        return rect;
    }

    #endregion

    #region MyRegion

    private void ShowEffectHeroOnly(List<GameObject> list)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Hero Only Effect: " + list.Count);

        GUILayout.FlexibleSpace();

        GUI.enabled = CanAddTrail(list);
        if (GUILayout.Button("+", width40))
        {
            AddTrail(list, null);
            //AddPoint3D(list, list.Count == 0 ? Vector3.zero : list[list.Count - 1]);
        }

        GUI.enabled = CanRemoveTrail(list);
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            RemoveTrail(list, list.Count - 1);
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();

            list[i] = (GameObject)EditorGUILayout.ObjectField("Effect only " + i, list[i], typeof(GameObject), true);

            GUI.enabled = CanAddTrail(list);
            if (GUILayout.Button("+", EditorStyles.miniButton, width40))
            {
                InsertTrail(list, i, list[i]);
            }
            GUI.enabled = CanRemoveTrail(list);
            if (GUILayout.Button("-", EditorStyles.miniButton, width40))
            {
                RemoveTrail(list, i);
            }
            GUI.enabled = true;
            //GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    #endregion

    #region 显示动画信息数据

    /// <summary>
    /// 显示特效动画信息
    /// </summary>
    /// <param name="anim_arr"></param>
    private Animator[] ShowEffectAnim(Animator[] anim_arr)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Effect Anim " + (anim_arr == null ? 0 : anim_arr.Length));

        GUILayout.FlexibleSpace();

        GUI.enabled = CanAddTrail<Animator>(anim_arr);
        if (GUILayout.Button("+", width40))
        {
            anim_arr = AddTrail(anim_arr, default(Animator));
            //AddPoint3D(list, list.Count == 0 ? Vector3.zero : list[list.Count - 1]);
        }

        GUI.enabled = CanRemoveTrail(anim_arr);
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            anim_arr = RemoveTrail(anim_arr, anim_arr.Length - 1);
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        if(anim_arr != null)
        {
            for (int i = 0; i < anim_arr.Length; i++)
            {
                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.BeginHorizontal();
                anim_arr[i] = (Animator)EditorGUILayout.ObjectField("Anim " + i, anim_arr[i], typeof(Animator), true);

                GUI.enabled = CanAddTrail(anim_arr);
                if (GUILayout.Button("+", EditorStyles.miniButton, width40))
                {
                    anim_arr = InsertTrail(anim_arr, i, anim_arr[i]);
                }
                GUI.enabled = CanRemoveTrail(anim_arr);
                if (GUILayout.Button("-", EditorStyles.miniButton, width40))
                {
                    anim_arr = RemoveTrail(anim_arr, i);
                }
                GUI.enabled = true;
                //GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }
        
        EditorGUILayout.EndVertical();

        return anim_arr;
    }

    /// <summary>
    /// 显示特效动画信息
    /// </summary>
    /// <param name="anim_arr"></param>
    private Animation[] ShowEffectAnimation(Animation[] anim_arr)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Effect Animation " + (anim_arr == null ? 0 : anim_arr.Length));

        GUILayout.FlexibleSpace();

        GUI.enabled = CanAddTrail<Animation>(anim_arr);
        if (GUILayout.Button("+", width40))
        {
            anim_arr = AddTrail(anim_arr, default(Animation));
            //AddPoint3D(list, list.Count == 0 ? Vector3.zero : list[list.Count - 1]);
        }

        GUI.enabled = CanRemoveTrail(anim_arr);
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            anim_arr = RemoveTrail(anim_arr, anim_arr.Length - 1);
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (anim_arr != null)
        {
            for (int i = 0; i < anim_arr.Length; i++)
            {
                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.BeginHorizontal();
                anim_arr[i] = (Animation)EditorGUILayout.ObjectField("Animation " + i, anim_arr[i], typeof(Animation), true);

                GUI.enabled = CanAddTrail(anim_arr);
                if (GUILayout.Button("+", EditorStyles.miniButton, width40))
                {
                    anim_arr = InsertTrail(anim_arr, i, anim_arr[i]);
                }
                GUI.enabled = CanRemoveTrail(anim_arr);
                if (GUILayout.Button("-", EditorStyles.miniButton, width40))
                {
                    anim_arr = RemoveTrail(anim_arr, i);
                }
                GUI.enabled = true;
                //GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndVertical();

        return anim_arr;
    }

    /// <summary>
    /// 检测是否可以添加点
    /// </summary>
    /// <returns></returns>
    bool CanAddTrail<T>(T[] arr)
    {
        return true;
    }

    /// <summary>
    /// 是否可以移除点
    /// </summary>
    /// <returns></returns>
    bool CanRemoveTrail<T>(T[] arr)
    {
        if (arr == null || arr.Length == 0)
            return false;

        return (arr.Length > 0);
    }

    /// <summary>
    /// 添加新的点
    /// </summary>
    /// <param name="v"></param>
    T[] AddTrail<T>(T[] arr, T anim)
    {
        if(arr == null)
        {
            arr = new T[1];
            arr[0] = anim;
        }
        else
        {
            List<T> list = new List<T>();
            list.AddRange(arr);
            list.Add(anim);

            arr = list.ToArray();
        }

        return arr;
    }

    /// <summary>
    /// 从中间插入新的点
    /// </summary>
    /// <param name="i"></param>
    /// <param name="v"></param>
    T[] InsertTrail<T>(T[] arr, int i, T t)
    {
        if (effect.FlyType == TSEffect1.FlyEnum.BEZIER && (i & 1) != 0)
            i--;

        if (t == null)
            return arr;

        if (arr == null)
        {
            arr = new T[1];
            arr[0] = t;
        }
        else
        {
            List<T> list = new List<T>();
            list.AddRange(arr);
            list.Insert(i, t);

            arr = list.ToArray();
        }
        return arr;
    }

    /// <summary>
    /// 移除一个点
    /// </summary>
    /// <param name="i"></param>
    T[] RemoveTrail<T>(T[] arr, int i)
    {
        if (effect.FlyType == TSEffect1.FlyEnum.BEZIER && (i & 1) != 0)
            i--;

        if (arr == null
            || arr.Length == 0
            || arr.Length <= i)
        {
            return arr;
        }
        else
        {
            List<T> list = new List<T>();
            list.AddRange(arr);
            list.RemoveAt(i);

            arr = list.ToArray();
        }
        return arr;
    }

    #endregion

    #region 显示粒子信息数据

    /// <summary>
    /// 显示特效粒子信息
    /// </summary>
    /// <param name="anim_arr"></param>
    private ParticleSystem[] ShowEffectParticleSystem(ParticleSystem[] particle_arr)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Effect ParticleSystem " + (particle_arr == null ? 0 : particle_arr.Length));

        GUILayout.FlexibleSpace();

        GUI.enabled = CanAddTrail<ParticleSystem>(particle_arr);
        if (GUILayout.Button("+", width40))
        {
            particle_arr = AddTrail(particle_arr, default(ParticleSystem));
            //AddPoint3D(list, list.Count == 0 ? Vector3.zero : list[list.Count - 1]);
        }

        GUI.enabled = CanRemoveTrail(particle_arr);
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            particle_arr = RemoveTrail(particle_arr, particle_arr.Length - 1);
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        if(particle_arr != null)
        {
            for (int i = 0; i < particle_arr.Length; i++)
            {
                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.BeginHorizontal();
                particle_arr[i] = (ParticleSystem)EditorGUILayout.ObjectField("PS " + i, particle_arr[i], typeof(ParticleSystem), true);

                GUI.enabled = CanAddTrail(particle_arr);
                if (GUILayout.Button("+", EditorStyles.miniButton, width40))
                {
                    particle_arr = InsertTrail(particle_arr, i, particle_arr[i]);
                }
                GUI.enabled = CanRemoveTrail(particle_arr);
                if (GUILayout.Button("-", EditorStyles.miniButton, width40))
                {
                    particle_arr = RemoveTrail(particle_arr, i);
                }
                GUI.enabled = true;
                //GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }
        
        EditorGUILayout.EndVertical();

        return particle_arr;
    }

    #endregion

    #region 显示 mesh renderer 数据信息

    /// <summary>
    /// 显示特效粒子信息
    /// </summary>
    /// <param name="anim_arr"></param>
    private Renderer[] ShowEffectParticleSystem(Renderer[] ren_arr)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Effect Renderer " + (ren_arr == null ? 0 : ren_arr.Length));

        GUILayout.FlexibleSpace();

        GUI.enabled = CanAddTrail<Renderer>(ren_arr);
        if (GUILayout.Button("+", width40))
        {
            ren_arr = AddTrail(ren_arr, default(Renderer));
            //AddPoint3D(list, list.Count == 0 ? Vector3.zero : list[list.Count - 1]);
        }

        GUI.enabled = CanRemoveTrail(ren_arr);
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            ren_arr = RemoveTrail(ren_arr, ren_arr.Length - 1);
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        if (ren_arr != null)
        {
            for (int i = 0; i < ren_arr.Length; i++)
            {
                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.BeginHorizontal();
                ren_arr[i] = (Renderer)EditorGUILayout.ObjectField("PS " + i, ren_arr[i], typeof(Renderer), true);

                GUI.enabled = CanAddTrail(ren_arr);
                if (GUILayout.Button("+", EditorStyles.miniButton, width40))
                {
                    ren_arr = InsertTrail(ren_arr, i, ren_arr[i]);
                }
                GUI.enabled = CanRemoveTrail(ren_arr);
                if (GUILayout.Button("-", EditorStyles.miniButton, width40))
                {
                    ren_arr = RemoveTrail(ren_arr, i);
                }
                GUI.enabled = true;
                //GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.EndVertical();

        return ren_arr;
    }

    #endregion

    #region 控制缩放对象数据

    /// <summary>
    /// 特效缩放对象
    /// </summary>
    /// <param name="list"></param>
    private void ShowEffectScale(List<Transform> list)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Scale Effect: " + list.Count);

        GUILayout.FlexibleSpace();

        GUI.enabled = CanAddObj(list);
        if (GUILayout.Button("+", width40))
        {
            AddObj(list, null);
            //AddPoint3D(list, list.Count == 0 ? Vector3.zero : list[list.Count - 1]);
        }

        GUI.enabled = CanRemoveObj(list);
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            RemoveObj(list, list.Count - 1);
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();

            list[i] = (Transform)EditorGUILayout.ObjectField("Scale Tran " + i, list[i], typeof(Transform), true);

            GUI.enabled = CanAddObj(list);
            if (GUILayout.Button("+", EditorStyles.miniButton, width40))
            {
                InsertObj(list, i, list[i]);
            }
            GUI.enabled = CanRemoveObj(list);
            if (GUILayout.Button("-", EditorStyles.miniButton, width40))
            {
                RemoveObj(list, i);
            }
            GUI.enabled = true;
            //GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 检测是否可以添加点
    /// </summary>
    /// <returns></returns>
    bool CanAddObj(List<Transform> list)
    {
        return true;
    }

    /// <summary>
    /// 是否可以移除点
    /// </summary>
    /// <returns></returns>
    bool CanRemoveObj(List<Transform> list)
    {
        return (list.Count > 0);
    }

    /// <summary>
    /// 添加新的点
    /// </summary>
    /// <param name="v"></param>
    void AddObj(List<Transform> list, Transform go)
    {
        list.Add(go);
    }

    /// <summary>
    /// 从中间插入新的点
    /// </summary>
    /// <param name="i"></param>
    /// <param name="v"></param>
    void InsertObj(List<Transform> list, int i, Transform go)
    {
        if (effect.FlyType == TSEffect1.FlyEnum.BEZIER && (i & 1) != 0)
            i--;

        list.Insert(i, go);
    }

    /// <summary>
    /// 移除一个点
    /// </summary>
    /// <param name="i"></param>
    void RemoveObj(List<Transform> list, int i)
    {
        if (effect.FlyType == TSEffect1.FlyEnum.BEZIER && (i & 1) != 0)
            i--;

        list.RemoveAt(i);
    }

    #endregion

    #region 显示拖尾对象数据

    /// <summary>
    /// 显示存储点数据列表信息
    /// </summary>
    TrailRenderer[] ShowEffectTrail(TrailRenderer[] trail_arr)
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("TrailRenderer " + (trail_arr == null ? 0 : trail_arr.Length));

        GUILayout.FlexibleSpace();

        GUI.enabled = CanAddTrail<TrailRenderer>(trail_arr);
        if (GUILayout.Button("+", width40))
        {
            trail_arr = AddTrail(trail_arr, default(TrailRenderer));
            //AddPoint3D(list, list.Count == 0 ? Vector3.zero : list[list.Count - 1]);
        }

        GUI.enabled = CanRemoveTrail(trail_arr);
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            trail_arr = RemoveTrail(trail_arr, trail_arr.Length - 1);
        }

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(5);

        if(trail_arr != null)
        {
            for (int i = 0; i < trail_arr.Length; i++)
            {
                EditorGUILayout.BeginVertical("Box");

                EditorGUILayout.BeginHorizontal();
                trail_arr[i] = (TrailRenderer)EditorGUILayout.ObjectField("Trail " + i, trail_arr[i], typeof(TrailRenderer), true);

                GUI.enabled = CanAddTrail(trail_arr);
                if (GUILayout.Button("+", EditorStyles.miniButton, width40))
                {
                    trail_arr = InsertTrail(trail_arr, i, trail_arr[i]);
                }
                GUI.enabled = CanRemoveTrail(trail_arr);
                if (GUILayout.Button("-", EditorStyles.miniButton, width40))
                {
                    trail_arr = RemoveTrail(trail_arr, i);
                }
                GUI.enabled = true;
                //GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
        }
        
        EditorGUILayout.EndVertical();

        return trail_arr;
    }

    /// <summary>
    /// 检测是否可以添加点
    /// </summary>
    /// <returns></returns>
    bool CanAddTrail(List<GameObject> list)
    {
        return true;
    }

    /// <summary>
    /// 是否可以移除点
    /// </summary>
    /// <returns></returns>
    bool CanRemoveTrail(List<GameObject> list)
    {
        return (list.Count > 0);
    }

    /// <summary>
    /// 添加新的点
    /// </summary>
    /// <param name="v"></param>
    void AddTrail(List<GameObject> list, GameObject go)
    {
        list.Add(go);
    }

    /// <summary>
    /// 从中间插入新的点
    /// </summary>
    /// <param name="i"></param>
    /// <param name="v"></param>
    void InsertTrail(List<GameObject> list, int i, GameObject go)
    {
        if (effect.FlyType == TSEffect1.FlyEnum.BEZIER && (i & 1) != 0)
            i--;

        list.Insert(i, go);
    }

    /// <summary>
    /// 移除一个点
    /// </summary>
    /// <param name="i"></param>
    void RemoveTrail(List<GameObject> list, int i)
    {
        if (effect.FlyType == TSEffect1.FlyEnum.BEZIER && (i & 1) != 0)
            i--;

        list.RemoveAt(i);
    }

    #endregion

    #region 显示贝塞尔曲线效果轨迹

    /// <summary>
    /// 显示贝塞尔轨迹信息数据
    /// </summary>
    void OnShowBezierPath()
    {
        if (effect.MTList.Count == 0)
        {
            Debug.LogError(" 没有曲线对象数据 ");
            return;
        }

        //显示轨迹
        for (int i = 0; i < effect.MTList.Count; i++)
        {
            OnShowBezierPath(i);
        }
    }

    /// <summary>
    /// 显示贝塞尔曲线路径点信息
    /// </summary>
    /// <param name="indexId"></param>
    void OnShowBezierPath(int indexId)
    {
        if (indexId < 0 || effect.MTList.Count < indexId
            || effect.MTList[indexId].points.Count == 0)
        {
            Debug.LogError(" 没有曲线数据 或者 没有曲线中间控制点 " + indexId);
            return;
        }

        //获取中间控制点
        List<Vector3> list = OnGetBezierPath(indexId);
        if (list == null || list.Count == 0)
            return;

        //显示中间控制点数据
        //创建父对象
        GameObject go = new GameObject("Bszier" + indexId);
        Transform parent = go.transform;

        UnityEngine.Color c = new UnityEngine.Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        for (int i = 0; i < list.Count; i++)
        {
            OnCreateShowObject(parent, i, list[i], c);
        }
    }

    /// <summary>
    /// 获取贝塞尔曲线中间点路径
    /// </summary>
    /// <param name="indexId"></param>
    /// <returns></returns>
    List<Vector3> OnGetBezierPath(int indexId)
    {
        if (indexId < 0 || effect.MTList.Count < indexId
            || effect.MTList[indexId].points.Count == 0)
        {
            Debug.LogError(" 没有曲线数据 或者 没有曲线中间控制点 ");
            return null;
        }

        List<Vector3> points = effect.MTList[indexId].points;
        List<Vector3> list = new List<Vector3>();

        Vector3 startPos = effect.transform.position;
        Vector3 targetPos = Vector3.zero;

        if (effect.Target != null)
            targetPos = effect.Target.transform.position;
        else
            targetPos = points[points.Count - 1] + startPos;

        //获取方向
        Vector3 dir = targetPos - startPos;
        Quaternion rot = Quaternion.LookRotation(dir);

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 v = new Vector3(points[i].x, points[i].y, points[i].z);
            v = startPos + rot * v;
            list.Add(v);
        }

        //插入头结点和尾结点
        list.Insert(0, startPos);
        if (!effect.IsOnlyCheckDis)
            list.Add(targetPos);

        //贝塞尔曲线控制时间
        float time = 0f;
        //中间位置控制点
        List<Vector3> tempPos = new List<Vector3>();
        Vector3 temp = Vector3.zero;
        for (int i = 0; i < 10; i++)
        {
            time += 0.1f;
            temp = OnDeCasteljau(list, list.Count - 1, 0, time);
            tempPos.Add(temp);
        }

        return tempPos;
    }

    /// <summary>
    /// deCasteljau算法 可以计算n阶贝塞尔曲线上的点的位置 比较消耗性能
    /// </summary>
    /// <param name="i">阶数</param>
    /// <param name="j">点</param>
    /// <param name="t">时间</param>
    /// <returns></returns>
    Vector3 OnDeCasteljau(List<Vector3> points, int i, int j, float t)
    {
        if (i == 1)
        {
            return (1 - t) * points[j] + t * points[j + 1];
        }
        return (1 - t) * OnDeCasteljau(points, i - 1, j, t) + t * OnDeCasteljau(points, i - 1, j + 1, t);
    }

    /// <summary>
    /// 创建一个显示对象数据
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="indexId"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    GameObject OnCreateShowObject(Transform parent, int indexId, Vector3 pos, UnityEngine.Color color)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = indexId.ToString();
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = Vector3.one;

        Renderer re = go.GetComponent<Renderer>();
        Material m = new Material(Shader.Find("Unlit/Color"));
        m.color = color;
        re.material = m;
        //re.material.SetColor("_Color", color);
        //go.GetComponent<Renderer>().material.color = color;

        return go;
    }

    #endregion

    #region 显示射线类型数据信息

    /// <summary>
    /// 显示移动对象列表数据
    /// </summary>
    void ShowRaysInfo()
    {
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Number of RayObjs: " + effect.arcList.Count);

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+", width40))
        {
            AddRayInfo();
        }
        GUI.enabled = CanRemoveRayInfo();
        GUILayout.Space(15);
        if (GUILayout.Button("-", width40))
        {
            RemoveRayInfo(effect.arcList.Count - 1);
        }
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        for (int i = 0; i < effect.arcList.Count; i++)
        {
            EditorGUILayout.BeginVertical("Box");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("    " + i, width60, height19);

            effect.arcList[i] = (ArcReactor_Arc)EditorGUILayout.ObjectField("Ray", effect.arcList[i], typeof(ArcReactor_Arc), true);

            if (GUILayout.Button("+", EditorStyles.miniButton, width40))
            {
                InsertRayInfo(i);
            }
            GUI.enabled = CanRemoveRayInfo();
            if (GUILayout.Button("-", EditorStyles.miniButton, width40))
            {
                RemoveRayInfo(i);
            }
            GUI.enabled = true;
            //GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 是否可以移除移动对象数据
    /// </summary>
    /// <returns></returns>
    bool CanRemoveMoveTranInfo()
    {
        return effect.MTList != null && effect.MTList.Count > 0;
    }

    /// <summary>
    /// 添加移动对象数据
    /// </summary>
    void AddMoveTranInfo()
    {
        //effect.MoveTransList.Add(null);
        //effect.PList.Add(new List<Vector3>());

        effect.MTList.Add(new TSEffect1.MoveTranInfo());
        if (effect.MTList.Count == 1)
        {
            //如果是第一个新添加的数据
            //那么设置默认值数据
            if (effect.FlyType == TSEffect1.FlyEnum.EJECTION
                || effect.FlyType == TSEffect1.FlyEnum.LIGHTNING_CHAIN)
                effect.MTList[0].tran = effect.Target.transform;
            else
                effect.MTList[0].tran = effect.transform;
            effect.MTList[0].points = effect.GetRandomPointList();
        }
    }

    /// <summary>
    /// 插入移动对象数据
    /// </summary>
    /// <param name="index"></param>
    void InsertMoveTranInfo(int index)
    {
        //effect.MoveTransList.Insert(index, null);
        //effect.PList.Insert(index, new List<Vector3>());

        effect.MTList.Insert(index, new TSEffect1.MoveTranInfo());
    }

    /// <summary>
    /// 移除移动对象数据
    /// </summary>
    /// <param name="index"></param>
    void RemoveMoveTranInfo(int index)
    {
        //effect.MoveTransList.RemoveAt(index);
        //effect.PList.RemoveAt(index);

        effect.MTList.RemoveAt(index);
    }

    #endregion

    #region 获取所有的存储信息数据

    /// <summary>
    /// 获取所有的拖尾对象
    /// </summary>
    /// <returns></returns>
    void UpdateAllSaveInfo()
    {
        effect.trail_arr = effect.GetComponentsInChildren<TrailRenderer>(true);
        effect.anim_arr = effect.GetComponentsInChildren<Animator>(true);
        effect.particle_arr = effect.GetComponentsInChildren<ParticleSystem>(true);
        effect.ren_arr = effect.GetComponentsInChildren<MeshRenderer>(true);
        effect.animation_arr = effect.GetComponentsInChildren<Animation>(true);
    }

    #endregion

}
