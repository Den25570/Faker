# Faker
User defined data type faker, capable of filling any complex data structure

## Usage
Declare **FakerConfig** and **Faker**:
```
FakerConfig config = new FakerConfig();
Faker faker = new Faker(config);
```
Add custom function to fake data if needed:
```
public class Foo
{
    public int FooInt;
}

public object CustomIntGenerator(Type[] genericTypes)
{
  return -1;
}

FakerConfig config = new FakerConfig();
config.Add<Foo, Int32>(CustomIntGenerator, obj => obj.FooInt);
Faker faker = new Faker(config);
```
Your custom function should return **object** and receive **Type[]**.
You can fill your custom class attribute if it has **public** access modifier or public constructor with the same name and type parameter.
Class declaration example:
```
public class Foo
{
    public int FooInt { get; private set; }
    public TestClosedClass(int fooInt)
    {
        FooInt = fooInt;
    }
}
```
Usage example:
```
public class Foo
{
    public int FooInt { get; private set; }
    public TestClosedClass(int fooInt)
    {
        FooInt = fooInt;
    }
}

public object CustomIntGenerator(Type[] genericTypes)
{
  Random rand = new Random();
  return rand.Next() % 10;
}

FakerConfig config = new FakerConfig();
config.Add<Foo, Int32>(CustomIntGenerator, obj => obj.FooInt);
Faker faker = new Faker(config);
Foo foo = (Foo)faker.Create(typeof(Foo));    
```
