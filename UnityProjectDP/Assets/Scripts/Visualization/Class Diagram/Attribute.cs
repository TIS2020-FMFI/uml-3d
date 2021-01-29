using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attribute
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string Id { get; set; }
    public string Value { get; set; }

    public Attribute(string id, string name, string type)
    {
        this.Id = id;
        this.Type = type;
        this.Name = name;
        if (Type == "int")
        {
            Value = "0";
        }
        else if (Type == "float")
        {
            Value = "0.0";
        }
        else
        {
            Value = "";
        }
    }
    public Attribute() { }

}
