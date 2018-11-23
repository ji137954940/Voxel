﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// 1、EditorApplication.update，这个是一个delegate，可以绑定一个函数，从而在编辑器下执行Update。
/// 2、EditorCoroutineRunner.StartEditorCoroutine(Routine1());  这样可以在编辑器下开启一个协程。
/// 2、另外一个思路是不使用协程，绑定一个Update函数，然后判断www.isDone来获取AssetBundle。这个我并没有实际验证。
/// 4、www可以正常的加载出AssetBundle，但是isDone的变量一直为false。额外要注意因为Editor模式下不存在退出游戏清理资源的概念，所以要注意处理已加载的assetbundle的情况，否则可能会报冲突的错误。
/// 5、理论上只支持yield return null这样的情况，延时要自己处理。Unity协程的原理是引擎在特定条件下执行MoveNext运行下面的语句，在上面的代码中不管是延时还是其他的东西，都是每帧执行MoveNext，这样WaitForSeconds这样的协程是无效的。  www的情况比较特殊，虽然理论上也是会有问题的，但是确实可以正常的取到结果。
/// </summary>

public static class EditorCoroutineRunner
{
    private class EditorCoroutine : IEnumerator
    {
        private Stack<IEnumerator> executionStack;

        public EditorCoroutine(IEnumerator iterator)
        {
            this.executionStack = new Stack<IEnumerator>();
            this.executionStack.Push(iterator);
        }

        public bool MoveNext()
        {
            IEnumerator i = this.executionStack.Peek();

            if (i.MoveNext())
            {
                object result = i.Current;
                if (result != null && result is IEnumerator)
                {
                    this.executionStack.Push((IEnumerator)result);
                }

                return true;
            }
            else
            {
                if (this.executionStack.Count > 1)
                {
                    this.executionStack.Pop();
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            throw new System.NotSupportedException("This Operation Is Not Supported.");
        }

        public object Current
        {
            get { return this.executionStack.Peek().Current; }
        }

        public bool Find(IEnumerator iterator)
        {
            return this.executionStack.Contains(iterator);
        }
    }

    private static List<EditorCoroutine> editorCoroutineList;
    private static List<IEnumerator> buffer;

    public static IEnumerator StartEditorCoroutine(IEnumerator iterator)
    {
        if (editorCoroutineList == null)
        {
            // test  
            editorCoroutineList = new List<EditorCoroutine>();
        }
        if (buffer == null)
        {
            buffer = new List<IEnumerator>();
        }
        if (editorCoroutineList.Count == 0)
        {
            EditorApplication.update += Update;
        }

        // add iterator to buffer first  
        buffer.Add(iterator);

        return iterator;
    }

    private static bool Find(IEnumerator iterator)
    {
        // If this iterator is already added  
        // Then ignore it this time  
        foreach (EditorCoroutine editorCoroutine in editorCoroutineList)
        {
            if (editorCoroutine.Find(iterator))
            {
                return true;
            }
        }

        return false;
    }

    private static void Update()
    {
        // EditorCoroutine execution may append new iterators to buffer  
        // Therefore we should run EditorCoroutine first  
        editorCoroutineList.RemoveAll
        (
            coroutine => { return coroutine.MoveNext() == false; }
        );

        // If we have iterators in buffer  
        if (buffer.Count > 0)
        {
            foreach (IEnumerator iterator in buffer)
            {
                // If this iterators not exists  
                if (!Find(iterator))
                {
                    // Added this as new EditorCoroutine  
                    editorCoroutineList.Add(new EditorCoroutine(iterator));
                }
            }

            // Clear buffer  
            buffer.Clear();
        }

        // If we have no running EditorCoroutine  
        // Stop calling update anymore  
        if (editorCoroutineList.Count == 0)
        {
            EditorApplication.update -= Update;
        }
    }
}