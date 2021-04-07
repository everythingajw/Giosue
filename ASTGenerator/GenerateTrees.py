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
ast_argument_group = parser.add_argument_group("AST")
ast_argument_group.add_argument("--generate-ast", dest="generate_ast", action="store_true",
                                required=False, help="Generate the AST")
ast_argument_group.add_argument("--ast-namespace", dest="ast_namespace", type=str, required=False,
                                help="The namespace that contains the trees.")
ast_argument_group.add_argument("--ast-output-dir", dest="ast_output_dir", type=str, required=False,
                                help="The location where the tree files should go.")

statement_argument_group = parser.add_argument_group("Statement")
statement_argument_group.add_argument("--generate-statement", dest="generate_statement", action="store_true",
                                      required=False, help="Generate the statement tree")
statement_argument_group.add_argument("--statement-namespace", dest="statement_namespace", type=str, required=False,
                                      help="The namespace that contains the trees.")
statement_argument_group.add_argument("--statement-output-dir", dest="statement_output_dir", type=str, required=False,
                                      help="The location where the tree files should go.")

args = parser.parse_args()

generate_ast = args.generate_ast
ast_output_dir = args.ast_output_dir
ast_namespace = args.ast_namespace
generate_statement = args.generate_statement
statement_namespace = args.statement_namespace
statement_output_dir = args.statement_output_dir

# Make sure the arguments are defined correctly
if generate_ast and (ast_output_dir is None or ast_namespace is None):
    print(f"Error: {ast_namespace.__name__} and {ast_output_dir.__name__} must be specified to generate the AST",
          file=sys.stderr)
    exit(1)
elif generate_statement and (statement_namespace is None or statement_output_dir is None):
    print(f"Error: {statement_namespace.__name__} and {statement_output_dir.__name__}" +
          " must be specified to generate the statement tree.", file=sys.stderr)
    exit(1)
else:
    # generate ast not specified and generate statement not specified
    print("Nothing to generate. Exit.", file=sys.stderr)
    exit(1)

ast_output_dir = Path(args.ast_output_dir).resolve()
statement_output_dir = Path(args.statement_output_dir).resolve()

exists_and_is_directory_or_exit(ast_output_dir)
exists_and_is_directory_or_exit(statement_output_dir)

if not prompt_yes_no(f"Writing AST to {ast_output_dir}. OK?"):
    print("Exit", file=sys.stderr)
    exit(3)
if not prompt_yes_no(f"Writing statement tree to {statement_output_dir}. OK?"):
    print("Exit", file=sys.stderr)
    exit(3)

prompt_to_remove_directory_contents(ast_output_dir)
prompt_to_remove_directory_contents(statement_output_dir)

# region AST

ast_namespace = str(ast_namespace).strip()
if len(ast_namespace) <= 0:
    print(f"Error: AST namespace is empty", file=sys.stderr)
    exit(1)

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

statement_tree_namespace = str(statement_namespace).strip()

if len(statement_tree_namespace) <= 0:
    print("Error: statement tree namespace is empty", file=sys.stderr)
    exit(1)

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

# Write the AST files
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
