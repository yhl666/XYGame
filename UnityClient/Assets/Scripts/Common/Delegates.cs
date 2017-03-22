/*
* Author:  caoshanshan
* Email:   me@dreamyouxi.com

 */
using UnityEngine;
using System.Collections;

public delegate void VoidFuncVoid();
public delegate void VoidFuncObject(object obj);
public delegate void VoidFuncString(string str);
public delegate void VoidFuncRpc(string msg, VoidFuncString cb);
public delegate object ObjectFuncVoid();

public delegate void VoidFunc3<T0, T1,T2>(T0 t0, T1 t1,T2 t2);
public delegate void VoidFunc2<T0, T1>(T0 t0, T1 t1);
public delegate void VoidFunc1<T0>(T0 t0);


public delegate void VoidFuncN<T0, T1, T2>(T0 t0, T1 t1, T2 t2);
public delegate void VoidFuncN<T0, T1>(T0 t0, T1 t1);
public delegate void VoidFuncN<T0>(T0 t0);
