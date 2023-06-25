using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;

namespace OpenDatabaseAPI;

public enum FieldType
{
    Bool,
    Int,
    Float,
    Double,
    Char,
    VarChar
}

public enum Flag
{
    NotNull,
    PrimaryKey,
    AlternateKey
}

public class Field
{
    public string Name;
    public FieldType Type;

    public int Size;
    
    public List<Flag> Flags;

    public static string[] FieldTypeStrings = new string[] {
        "BOOL",
        "INT",
        "FLOAT",
        "DOUBLE",
        "CHAR",
        "VARCHAR"
    };

    public static string[] FlagStrings = new string[] {
        "NOT NULL",
        "PRIMARY KEY",
        "UNIQUE"
    };

    public void AddFlag(Flag flag)
    {
        this.Flags.Add(flag);
    }

    protected void Initialize(Flag[] flags)
    {
        for (int x = 0; x < flags.Length; x++)
            this.Flags.Add(flags[x]);
    }

    protected string GetFlagString()
    {
        if (this.Flags.Count == 0)
            return null;
        
        StringBuilder stringBuilder = new StringBuilder();
        
        for (int x = 0; x < this.Flags.Count - 1; x++)
        {
            stringBuilder.Append(Field.FlagStrings[(int)this.Flags[x]]);
            stringBuilder.Append(' ');
        }
        
        stringBuilder.Append(Field.FlagStrings[(int)this.Flags[this.Flags.Count - 1]]);

        return stringBuilder.ToString();
    }

    public override string ToString()
    {
        bool hasSize = this.Size > 0;
      
        return $"{this.Name} {Field.FieldTypeStrings[(int)this.Type]}{((hasSize) ? '(' : null)}{((hasSize) ? Size : null)}{((hasSize) ? ')' : null)} {this.GetFlagString()}";
    }

    public Field(string name, FieldType type, Flag[] flags = null, int size = 0)
    {
        this.Name = name;
        this.Type = type;
        this.Size = size;

        this.Flags = new List<Flag>();
    
        if (flags != null)
            this.Initialize(flags);
    }
} 
