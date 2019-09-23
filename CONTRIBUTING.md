#Contribution

##Coding standards:
For this project, we follow the coding standards as presented by the unity guildelines: 
https://wiki.unity3d.com/index.php/Csharp_Coding_Guidelines

Additionally, we follow the following script organization rules:

1. Members of the class are on top of the class definition. They are organized in the following order: public, protected, private.
2. Methods of the class are bellow the members definition. They are organized in the following order: public, protected, private. 
They are also ordered alphabetically, within in each modifier.
3. Private memeber variable has the prefix of an underscore.
```csharp
public class SomeClass
{
  public float speed;
  
  private float _width;
  
  public float width;
  {
    get
    {
      return _width;
    }
    set
    {
      _width = value;
    }
  }
}
```

##Unity files naming standards:

##Pull request standards:
