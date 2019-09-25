# Contribution

## Coding standards
For this project, we follow the [Unity guidelines](https://wiki.unity3d.com/index.php/Csharp_Coding_Guidelines) for coding, with the following additions:
- Four spaces are used for indentation.
- Lines cannot be longer than 100 characters.
- Otherwise empty lines must not contain any leftover indentation.
- Every file must end with a newline character.
- Files uploaded to the repository must not overwrite existing line ending configurations (_LF_ or _CRLF_).

Additionally, we follow the following script organization rules for classes:

1. Constants.
1. Member variables, organized in the following order: `public`, `protected`, `private`.
2. Methods, including getters and setters, organized in the following order: `public`, `protected`, `private`. Additionally, within each set of methods corresponding to a particular modifier, methods are ordered alphabetically.
3. The identifiers of _private_ memeber variable begin with `_`.
4. Comments are used for every public method, using JavaDoc style as one of the options provided by Doxygen documentation generator.

Example:
```csharp
public class SomeClass
{
    public float speed;
  
    private float _width;

    /**
     * @brief Width of something.
     */
    public float Width
    {
        get { return _width; }
        set { _width = value; }
    }
}
```

## Unity file naming standards
...

## Pull request standards
...
