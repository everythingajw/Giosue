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
import shutil
from common import *
from field import Field
from syntax_tree import SyntaxTree

parser = argparse.ArgumentParser(description='Generate the ASTs for Giosue.')
parser.add_argument("--ast-namespace", dest="ast_namespace", type=str, required=True,
                    help="The namespace that contains the trees.")
parser.add_argument("--ast-output-dir", dest="ast_output_dir", type=str, required=True,
                    help="The location where the tree files should go.")
parser.add_argument("--statement-namespace", dest="statement_namespace", type=str, required=True,
                    help="The namespace that contains the trees.")
parser.add_argument("--statement-output-dir", dest="statement_output_dir", type=str, required=True,
                    help="The location where the tree files should go.")

args = parser.parse_args()

ast_output_dir = Path(args.ast_output_dir).resolve()
statement_output_dir = Path(args.statement_output_dir).resolve()

exists_and_is_directory_or_exit(ast_output_dir)
exists_and_is_directory_or_exit(statement_output_dir)

if not prompt_yes_no(f"Writing AST to {ast_output_dir}. OK?"):
    print("Exit", file=sys.stderr)
    exit(3)

if len(os.listdir(str(ast_output_dir))) != 0:
    print("Warning: output directory is not empty", file=sys.stderr)

    if not prompt_yes_no("Remove contents before continuing?"):
        print("Keeping directory contents")

    # Response is yes
    else:
        print("Removing directory contents", file=sys.stderr)
        for file in ast_output_dir.rglob("*"):
            if file.is_dir():
                if prompt_yes_no(f"{file} is a directory. Recursively remove?"):
                    print("Removing directory")
                    shutil.rmtree(str(file))
                else:
                    print("Keeping directory")
            else:
                file.unlink()

# region AST

ast_namespace = str(args.ast_namespace).strip()

using_statements: List[str] = list(
    map(lambda u: f"{u}\n",
        [
            "using System;",
            "using System.Collections.Generic;",
            "using System.Linq;",
            "using System.Text;",
            "using Giosue;",
            f"using {ast_namespace};"
        ]
        )
)

syntax_trees: List[SyntaxTree] = [
    SyntaxTree(
        ast_namespace,
        "Assign",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Token", "Name", "name"),
            Field("Expression", "Value", "@value")
        ]
    ),
    SyntaxTree(
        ast_namespace,
        "Binary",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Expression", "Left", "left"),
            Field("Token", "Operator", "@operator"),
            Field("Expression", "Right", "right"),
        ],
    ),
    SyntaxTree(
        ast_namespace,
        "Call",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Expression", "Callee", "callee"),
            Field("Token", "Paren", "paren"),
            Field("List<Expression>", "Arguments", "arguments"),
        ],
    ),
    SyntaxTree(
        ast_namespace,
        "Get",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Expression", "Object", "@object"),
            Field("Token", "Name", "name"),
        ],
    ),
    SyntaxTree(
        ast_namespace,
        "Grouping",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Expression", "Expression", "expression"),
        ],
    ),
    SyntaxTree(
        ast_namespace,
        "Literal",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("object", "Value", "@value"),
        ],
    ),
    SyntaxTree(
        ast_namespace,
        "Logical",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Expression", "Left", "left"),
            Field("Token", "Operator", "@operator"),
            Field("Expression", "Right", "right"),
        ],
    ),
    SyntaxTree(
        ast_namespace,
        "Set",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Expression", "Object", "@object"),
            Field("Token", "Name", "name"),
            Field("Expression", "Value", "@value"),
        ],
    ),
    SyntaxTree(
        ast_namespace,
        "Super",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Token", "Keyword", "keyword"),
            Field("Token", "Method", "method"),
        ],
    ),
    SyntaxTree(
        ast_namespace,
        "This",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Token", "Keyword", "keyword"),
        ],
    ),
    SyntaxTree(
        ast_namespace,
        "Unary",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Token", "Operator", "@operator"),
            Field("Expression", "Right", "right"),
        ],
    ),
    SyntaxTree(
        ast_namespace,
        "Variable",
        BASE_EXPRESSION_CLASS_NAME,
        [
            Field("Token", "Name", "name"),
        ],
    ),
]

ast_visitor_interface_methods = [
    f"{GENERIC_PARAMETER} Visit{tree.name}{tree.base_class_name}({tree.name} expression);" for tree in syntax_trees
]

ast_visitor_interface = "\n".join(
    add_namespace(
        [
            f"public interface {VISITOR_INTERFACE_NAME}",
            "{",
            *indent(ast_visitor_interface_methods),
            "}"
        ],
        ast_namespace
    )
)

ast_base_class_methods = [
    f"public abstract {GENERIC_PARAMETER} Accept<{GENERIC_PARAMETER}>({VISITOR_INTERFACE_NAME} visitor);"]
ast_base_class = "\n".join(
    add_namespace(
        [
            f"public abstract class {BASE_EXPRESSION_CLASS_NAME}",
            "{",
            *indent(ast_base_class_methods),
            "}"
        ],
        ast_namespace
    )
)

# endregion AST

# region Statement trees

statement_tree_namespace = str(args.statement_namespace).strip()

statement_trees: List[SyntaxTree] = [
    SyntaxTree(
        statement_tree_namespace,
        "Expression",
        BASE_STATEMENT_CLASS_NAME,
        [
            Field("Expression", "Expression", "expression")
        ]
    ),
]

statement_visitor_interface_methods = []
statement_visitor_interface = add_namespace([
    f"public interface {VISITOR_INTERFACE_NAME}",
    "{",
    *indent(statement_visitor_interface_methods),
    "}"
], statement_tree_namespace)

# endregion Statement trees

with open(ast_output_dir / "IVisitor.cs", "w") as f:
    f.writelines(using_statements)
    f.write("\n")
    f.write(ast_visitor_interface)

with open(ast_output_dir / "Expression.cs", "w") as f:
    f.writelines(using_statements)
    f.write("\n")
    f.write(ast_base_class)

for tree in syntax_trees:
    output_file_path = ast_output_dir / f"{tree.name}.cs"
    with open(output_file_path, "w") as f:
        f.writelines(using_statements)
        f.write("\n")
        f.write(tree.generate_tree())

print("OK.", file=sys.stderr)
