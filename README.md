Overview
============
This application turns equations into a simplified canonical form.

Examples:
* "x^2 + 3.5xy + y = y^2 - xy + y" will transform into "x^2 - y^2 + 4.5xy = 0" (or a equivalent equation)
* "x = 1" into "x - 1 = 0"
* "x - (y^2 - x) = 0" into "2x - y^2 = 0"
* "x - (0 - (0 - x)) = 0" into"0 = 0"


To do this, it tokenizes on numbers, summands, brackets and (+, -, *) operators.
Turns such tokens into postfix form, solving precedence and finally simplifies
and reassembles it as a canonical equation.

*This program was completed in C# using .NET Framework 4 through Visual Studio 2017.

Run
================
The program supports two modes of operation: 
1. “interactive” where equations can be provided in the console
2. "file" where a file with equations is provided as a parameter


* If interactive mode is used and the given line is invalid, "invalid input" will be written in the console
* If file mode is used and one of the lines in a given file is invalid, "invalid input" will be written in the output file


Notes
================
* Input is assumed to be valid arrangement of valid letters, numbers, and operators. 
* Parentheses are assumed to should be in pairs and the right order
* The program can be improved on by doing some further input error checking

* Input will be explicit in multiplication (ie. (x+2)(x+2) = 0 is not valid input)
* It is assumed that only input with addition, subtraction, multiplication, and exponent signs will be given.
* Summands can contain the same variable multiple times (ie. 2x^2yx^2 = 3)
* Numbers can only contain digits, commas and decimal marks
* Variables can be written in any order, (ie. "2xy + yx = 3" will give output "3xy - 3 = 0")
* Exponent bases cant be numbers and must be variables
* Exponents must be positive and must be an integer
