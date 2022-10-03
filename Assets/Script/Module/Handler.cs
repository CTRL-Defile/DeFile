namespace Module
{
    public delegate void Handler();
    public delegate void Handler<T0>(T0 arg0);
    public delegate void Handler<T0, T1>(T0 arg0, T1 arg1);
    public delegate void Handler<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);
    public delegate void Handler<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3);
    public delegate void Handler<T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate void Handler<T0, T1, T2, T3, T4, T5>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate void Handler<T0, T1, T2, T3, T4, T5, T6>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    //return타입도 추가해 놓자.
    public delegate R0 HandlerV2<R0>();
    public delegate R0 HandlerV2<R0, T0>(T0 arg0);
    public delegate R0 HandlerV2<R0, T0, T1>(T0 arg0, T1 arg1);
    public delegate R0 HandlerV2<R0, T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2);
    public delegate R0 HandlerV2<R0, T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3);
    public delegate R0 HandlerV2<R0, T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate R0 HandlerV2<R0, T0, T1, T2, T3, T4, T5>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
}