using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType {NONE, GREEN, YELLOW, PINK}

public static class DamageClassColor {
    public static Color GetColor(DamageType damageType) {
        switch (damageType) {
            case DamageType.GREEN :
                return Color.green;
            case DamageType.YELLOW :
                return Color.yellow;
            case DamageType.PINK :
                return new Color(1,0,1,1);
            default :
                return Color.gray;
        }
    }
}

/*public class DamageTypeClass
{
    public static DamageTypeClass NONE {get;} = new DamageTypeClass(0, Color.gray);
    public static DamageTypeClass GREEN {get;} = new DamageTypeClass(1, Color.green);
    public static DamageTypeClass YELLOW {get;} = new DamageTypeClass(2, Color.yellow);
    public static DamageTypeClass PINK {get;} = new DamageTypeClass(3, new Color(1,0,1,1));

    public DamageTypeClass(int id, Color color) {
        _id = id;
        _color = color;
    }

    public int _id {get;}
    public Color _color {get;}

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        return ((DamageTypeClass)obj)._id == this._id;
    }

    public static bool operator ==(DamageTypeClass c1, DamageTypeClass c2) 
    {
        return c1.Equals(c2);
    }

    public static bool operator !=(DamageTypeClass c1, DamageTypeClass c2) 
    {
    return !c1.Equals(c2);
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }
}*/
