//Data structure for single animation

public struct MethodBody
{
    public string Code { set; get; }
    public string MethodName { set; get; }
    public MethodBody(string method_name, string code)
    {
        Code = code;
        MethodName = method_name;
    }
    public MethodBody(string method_name)
    {
        MethodName = method_name;
        Code = null;
    }

}
