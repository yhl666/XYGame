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

