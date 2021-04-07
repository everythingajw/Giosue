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
from common import *
from field import Field
from syntax_tree import SyntaxTree

parser = argparse.ArgumentParser(description='Generate the ASTs for Giosue.')

parser.add_argument("--namespace", dest="namespace", type=str, required=True,
                    help="The namespace that contains the trees.")

parser.add_argument('--ast-output-dir', dest="ast_output_dir", type=str, required=True,
                    help="The location where the tree files should go.")

args = parser.parse_args()

tree_namespace = str(args.namespace).strip()

using_statements: List[str] = list(
    map(lambda u: f"{u}\n",
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

ast_output_dir = Path(args.ast_output_dir).resolve()

if not ast_output_dir.exists():
    print("Error: the output directory does not exist.", file=sys.stderr)
    exit(1)
elif not ast_output_dir.is_dir():
    print("Error: the output directory is not a directory.", file=sys.stderr)
    exit(2)

response = None
while response != "" and response not in ALL_YES_NO_RESPONSES:
    print(f"Writing AST to {ast_output_dir}. OK? (y/N) ", file=sys.stderr, end='')
    response = input().strip().lower()

if response == "" or response in NO_RESPONSES:
    print("Exit", file=sys.stderr)
    exit(3)

if len(os.listdir(str(ast_output_dir))) != 0:
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
        for file in ast_output_dir.rglob("*"):
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

base_class_methods = [
    f"public abstract {GENERIC_PARAMETER} Accept<{GENERIC_PARAMETER}>({VISITOR_INTERFACE_NAME} visitor);"]
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

with open(ast_output_dir / "IVisitor.cs", "w") as f:
    f.writelines(using_statements)
    f.write("\n")
    f.write(visitor_interface)

with open(ast_output_dir / "Expression.cs", "w") as f:
    f.writelines(using_statements)
    f.write("\n")
    f.write(base_class)

for tree in syntax_trees:
    output_file_path = ast_output_dir / f"{tree.name}.cs"
    with open(output_file_path, "w") as f:
        f.writelines(using_statements)
        f.write("\n")
        f.write(tree.generate_tree())

print("OK.", file=sys.stderr)
