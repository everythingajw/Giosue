# Language Reference

- A Giosue program is executed line by line, from top to bottom, just like a Python program.
- All statements are semicolon-terminated.
- Comments begin with `--` and run until a newline is encountered.
- A valid identifier is not a reserved keyword. It begins with a letter or an underscore, can contain numbers after the first character, and is at least one character long.

## Data Types

- Giosue supports 32-bit (4 byte) integers, 64-bit (8 byte) floats, booleans, and strings.
- Strings are double-quoted and are completely raw. There are no escapes of any kind.
- Floats must have digits on both sides of the decimal point. `0.4` is valid, but `.4` is invalid.

## Operators

- Giosue is very strict. You must use the same data types on both sides of an operator. Thus, `1 + 1` is valid, but `1 + 1.0` is invalid.
  - The exception is `==` and `!=`. They can be used with any type on either side. If the values on each side of `==` or `!=` are not the same type, they are never considered equal.
- `+` for integer and floating point addition.
  - Adding integers results in an integer, adding floats results in a float.
- `-` for integer and floating point subtraction.
  - Subtracting integers results in an integer, subtracting floats results in a float.
- `*` for integer and floating point multiplication.
  - Multiplying integers results in an integer, multiplying floats results in a float.
- `/` for integer and floating point division.
  - Unlike the other operators, this always results in a float.
- `&` for integer bitwise and.
  - This operator does not work with floats and always results in an int.
- `|` for integer bitwise or.
  - This operator does not work with floats and always results in an int.
- `^` for integer bitwise xor.
  - This operator does not work with floats and always results in an int.
- `&&` for boolean and.
  - Only works with booleans.
- `||` for boolean or.
  - Only works with booleans.
- `^^` for boolean xor.
  - Only works with booleans.
- `!` for boolean not.
  - Only works with booleans.
- `==` for equality.
  - This operator works regardless of the types on the sides of the expression. Values of different types are never considered equal.
- `!=` for not equals.
  - This operator works regardless of the types on the sides of the expression. Values of different types are never considered equal.
- `<` for less than.
  - This operator only compares integer to integer or float to float.
- `>` for greater than.
  - This operator only compares integer to integer or float to float.
- `<=` for less than or equal to.
  - This operator only compares integer to integer or float to float. It does not work with all types like `==` does because you're still checking to see if one value is less than another.
- `>=` for greater than or equal to.
  - This operator only compares integer to integer or float to float. It does not work with all types like `==` does because you're still checking to see if one value is greater than another.

## Variables

- Variables are declared with the `var` keyword.
- The type of the variable is inferred from the right hand side of the expression.

```text
var myInteger = 1;
var myFloat = 1.0;
var myBool = vero;
var myString = "cool string";
```

### Augmented assignment

- Augmented assignment is not supported.
- Increment/decrement by one (`i++` and `i--` in C) is not supported.

```text
var x = 1;

-- Invalid
-- x += 5;

-- Also invalid
-- x++;
-- x--;

-- Valid
x = x + 5;
```

## Functions

- Functions are declared with the `fun` keyword. They can have any number of parameters.
- At this time, functions cannot return a value.

```text
fun functionWithZeroParameters() 
{
    ScriveLina("No parameters to see here!");
}

fun functionWithOneParameter(p)
{
    ScriveLina("The parameter is: " @ TrasformaInStringa(p));
}

fun functionWithTwoParameters(p1, p2)
{
    ScriveLina("The first parameter is: " @ TrasformaInStringa(p1));
    ScriveLina("The second parameter is: " @ TrasformaInStringa(p2));
}
```

## Control flow

### If statements

- There are only two possible boolean values: `vero` (`true`)and `falso` (`false`).
- No automatic casting is performed, therefore `1` is not a valid condition.

```text
se (vero) 
{
    ScriveLina("Condition was true");
}
else 
{
    ScriveLina("Condition was false");
}
```

### While loops

- Again, there are only two possible boolean values: `vero` and `falso`.
- There are no break or continue statements. A `mentre (vero)` loop will run forever until an error is encountered, the program crashes, or the user manually stops the program.

```text
mentre (vero)
{
    ScriveLina("Running forever and ever and ever and ever!");
}
```
