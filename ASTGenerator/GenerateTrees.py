# defineAst(outputDir, "Expr", Arrays.asList(
#     "Binary   : Expr left, Token operator, Expr right",
#     "Grouping : Expr expression",
#     "Literal  : Object value",
#     "Unary    : Token operator, Expr right"
# ));

from typing import *
from pathlib import Path
import argparse
import sys
import os

INDENT = "    "
BASE_EXPRESSION_CLASS_NAME = "Expression"
GENERIC_PARAMETER = "T"
VISITOR_INTERFACE_NAME = f"IVisitor<{GENERIC_PARAMETER}>"
YES_RESPONSES = ("yes", "y")
NO_RESPONSES = ("no", "n")
ALL_YES_NO_RESPONSES = (*YES_RESPONSES, *NO_RESPONSES)


def indent(block: List[str], indent = INDENT):  # -> List[str]:
    return list(map(lambda f: indent + f, block))


def add_namespace(block, name):
    return [
        f"namespace {name}",
        "{",
        *indent(block),
        "}"
    ]


class Field:
    def __init__(self, type_name: str, field_name: str, constructor_parameter_name: str):
        self.type_name: str = type_name
        self.field_name: str = field_name
        self.constructor_parameter_name: str = constructor_parameter_name

    def get_constructor_parameter(self):
        return f"{self.type_name} {self.constructor_parameter_name}"

    def get_initializer(self):
        return f"this.{self.field_name} = {self.constructor_parameter_name};"

    def get_field_name(self):
        return f"public {self.type_name} {self.field_name} " + "{ get; }"


class SyntaxTree:
    def __init__(self, namespace: str, name: str, base_class_name: str, fields: List[Field]):
        self.namespace: str = namespace
        self.name: str = name
        self.base_class_name: str = base_class_name
        self.fields: List[Field] = fields

    def generate_constructor_parameters(self):
        return [field.get_constructor_parameter() for field in self.fields]
    
    def generate_fields(self):
        return [field.get_field_name() for field in self.fields]

    def generate_field_initializers(self):
        return [field.get_initializer() for field in self.fields]
    
    def generate_visitor_pattern(self):
        visitor_call = indent([f"return visitor.Visit{self.name}{self.base_class_name}(this);"])
        
        return [
            f"public override {GENERIC_PARAMETER} Accept<{GENERIC_PARAMETER}>({VISITOR_INTERFACE_NAME} visitor)",
            "{",
            *visitor_call,
            "}"
        ]
    
    def generate_constructor(self):
        parameters = ", ".join(self.generate_constructor_parameters())
        initializers = indent(self.generate_field_initializers())

        return [
            f"public {self.name}({parameters})",
            "{",
            *initializers,
            "}"
        ]

    def generate_class(self):
        fields = indent(self.generate_fields())
        constructor = indent(self.generate_constructor())
        visitor = indent(self.generate_visitor_pattern())

        return [
            f"public class {self.name} : {self.base_class_name}",
            "{",
            *fields,
            "",
            *constructor,
            "",
            *visitor,
            "}"
        ]

    def generate_namespace(self):
        class_ = indent(self.generate_class())

        return [
            f"namespace {self.namespace}",
            "{",
            *class_,
            "}"
        ]

    def generate_tree(self):
        return "\n".join(self.generate_namespace())


parser = argparse.ArgumentParser(description='Generate the ASTs for Giosue.')

parser.add_argument("--namespace", dest="namespace", type=str, required=True,
                    help="The namespace that contains the trees.")

parser.add_argument('--output-dir', dest="output_dir", type=str, required=True, 
                    help="The location where the tree files should go.")

args = parser.parse_args()

tree_namespace = str(args.namespace).strip()

using_statements: List[str] = list(
    map(lambda u : f"{u}\n", 
        [
            "using System;",
            "using System.Collections.Generic;",
            "using System.Linq;",
            "using System.Text;",
            "using Giosue;",
            f"using {tree_namespace};"
        ]
    )
)

output_dir = Path(args.output_dir).resolve()

if not output_dir.exists():
    print("Error: the output directory does not exist.", file=sys.stderr)
    exit(1)
elif not output_dir.is_dir():
    print("Error: the output directory is not a directory.", file=sys.stderr)
    exit(2)

response = None
while response != "" and response not in ALL_YES_NO_RESPONSES:
    print(f"Writing AST to {output_dir}. OK? (y/N) ", file=sys.stderr, end='')
    response = input().strip().lower()

if response == "" or response in NO_RESPONSES:
    print("Exit", file=sys.stderr)
    exit(3)

if len(os.listdir(str(output_dir))) != 0:
    print("Warning: output directory is not empty", file=sys.stderr)
    response = None
    while response != "" and response not in ALL_YES_NO_RESPONSES:
        print("Remove contents before continuing? (y/N) ", file=sys.stderr, end="")
        response = input().strip().lower()
    
    if response == "" or response in NO_RESPONSES:
        print("Keeping directory contents", file=sys.stderr)
    
    # Response is yes
    else:
        print("Removing directory contents", file=sys.stderr)
        for file in output_dir.rglob("*"):
            if file.is_dir():
                response = None
                while response != "" and response not in ALL_YES_NO_RESPONSES:
                    print(f"{file} is a directory. Remove anyway? (y/N) ", file=sys.stderr, end="")
                    response = input().strip().lower()
                if response != "" and response in YES_RESPONSES:
                    file.rmdir()
            else:
                file.unlink()

syntax_trees: List[SyntaxTree] = [
    SyntaxTree(
        tree_namespace, 
        "Assign", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Token", "Name", "name"),
            Field("Expression", "Value", "@value")
        ]
    ),
    SyntaxTree(
        tree_namespace, 
        "Binary", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Expression", "Left", "left"),
            Field("Token", "Operator", "@operator"),
            Field("Expression", "Right", "right"),
        ],
    ),
    SyntaxTree(
        tree_namespace, 
        "Call", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Expression", "Callee", "callee"),
            Field("Token", "Paren", "paren"),
            Field("List<Expression>", "Arguments", "arguments"),
        ],
    ),
    SyntaxTree(
        tree_namespace, 
        "Get", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Expression", "Object", "@object"),
            Field("Token", "Name", "name"),
        ],
    ),
    SyntaxTree(
        tree_namespace, 
        "Grouping", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Expression", "Expression", "expression"),
        ],
    ),
    SyntaxTree(
        tree_namespace, 
        "Literal", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("object", "Value", "@value"),
        ],
    ),
    SyntaxTree(
        tree_namespace, 
        "Logical", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Expression", "Left", "left"),
            Field("Token", "Operator", "@operator"),
            Field("Expression", "Right", "right"),
        ],
    ),
    SyntaxTree(
        tree_namespace, 
        "Set", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Expression", "Object", "@object"),
            Field("Token", "Name", "name"),
            Field("Expression", "Value", "@value"),
        ],
    ),
    SyntaxTree(
        tree_namespace, 
        "Super", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Token", "Keyword", "keyword"),
            Field("Token", "Method", "method"),
        ],
    ),
    SyntaxTree(
        tree_namespace, 
        "This", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Token", "Keyword", "keyword"),
        ],
    ),
    SyntaxTree(
        tree_namespace, 
        "Unary", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Token", "Operator", "@operator"),
            Field("Expression", "Right", "right"),
        ],
    ),
    SyntaxTree(
        tree_namespace, 
        "Variable", 
        BASE_EXPRESSION_CLASS_NAME, 
        [
            Field("Token", "Name", "name"),
        ],
    ),
]

visitor_interface_methods = [
    f"{GENERIC_PARAMETER} Visit{tree.name}{tree.base_class_name}({tree.name} expression);" for tree in syntax_trees
]

visitor_interface = "\n".join(
    add_namespace(
        [
            f"public interface {VISITOR_INTERFACE_NAME}",
            "{",
            *indent(visitor_interface_methods),
            "}"
        ],
        tree_namespace
    )
)

base_class_methods = [f"public abstract {GENERIC_PARAMETER} Accept<{GENERIC_PARAMETER}>({VISITOR_INTERFACE_NAME} visitor);"]
base_class = "\n".join(
    add_namespace(
        [
            f"public abstract class {BASE_EXPRESSION_CLASS_NAME}",
            "{",
            *indent(base_class_methods),
            "}"
        ],
        tree_namespace
    )
)

with open(output_dir / "IVisitor.cs", "w") as f:
    f.writelines(using_statements)
    f.write("\n")
    f.write(visitor_interface)

with open(output_dir / "Expression.cs", "w") as f:
    f.writelines(using_statements)
    f.write("\n")
    f.write(base_class)

for tree in syntax_trees:
    output_file_path = output_dir / f"{tree.name}.cs"
    with open(output_file_path, "w") as f:
        f.writelines(using_statements)
        f.write("\n")
        f.write(tree.generate_tree())

print("OK.", file=sys.stderr)