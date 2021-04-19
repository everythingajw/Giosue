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

if not generate_ast and not generate_statement:
    print("Nothing to generate. Exit.", file=sys.stderr)
    exit(1)

# Make sure the arguments are defined correctly
if generate_ast and (ast_output_dir is None or ast_namespace is None):
    print(f"Error: AST namespace and AST output directory must be specified to generate the AST",
          file=sys.stderr)
    exit(1)
elif generate_statement and (statement_namespace is None or statement_output_dir is None):
    print(
        f"Error: statement namespace and statement output directory must be specified to generate the statement tree.",
        file=sys.stderr)
    exit(1)


def write_tree_to_file(output_dir: Path, confirmation_prompt: str, namespace: str, using_statements: List[str],
                       visitor_interface_method_parameter: str, trees: List[SyntaxTree], base_class_name: str):
    output_dir = output_dir.resolve()
    exists_and_is_directory_or_exit(output_dir)

    if not prompt_yes_no(confirmation_prompt):
        print("Skip", file=sys.stderr)
        return

    prompt_to_remove_directory_contents(output_dir)

    visitor_interface_methods = [
        f"public {GENERIC_PARAMETER} Visit{tree.name}{tree.base_class_name}({tree.name} {visitor_interface_method_parameter});"
        for tree in trees
    ]

    visitor_interface = add_namespace([
        f"public interface {VISITOR_INTERFACE_NAME}",
        "{",
        *indent(visitor_interface_methods),
        "}"
    ], namespace)

    base_class_methods = [
        f"public abstract {GENERIC_PARAMETER} Accept<{GENERIC_PARAMETER}>({VISITOR_INTERFACE_NAME} visitor);"
    ]

    base_class = add_namespace([
        f"public abstract class {base_class_name}",
        "{",
        *indent(base_class_methods),
        "}"
    ], namespace)

    using_statements = "\n".join(using_statements)

    with open(output_dir / "IVisitor.cs", "w") as f:
        f.writelines(using_statements)
        f.write("\n\n")
        f.write("\n".join(visitor_interface))

    with open(output_dir / f"{base_class_name}.cs", "w") as f:
        f.writelines(using_statements)
        f.write("\n\n")
        f.write("\n".join(base_class))

    for tree in trees:
        output_file_path = output_dir / f"{tree.name}.cs"
        with open(output_file_path, "w") as f:
            f.writelines(using_statements)
            f.write("\n\n")
            f.write(tree.generate_tree())


if generate_ast:
    ast_namespace = str(ast_namespace).strip()
    if len(ast_namespace) <= 0:
        print(f"Error: AST namespace is empty", file=sys.stderr)
        exit(1)

    statement_using_statements: List[str] = [
        "using System;",
        "using System.Collections.Generic;",
        "using System.Linq;",
        "using System.Text;",
        "using Giosue;",
        f"using {ast_namespace};"
    ]

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

    ast_output_dir = Path(ast_output_dir)
    write_tree_to_file(ast_output_dir, f"Writing AST to {ast_output_dir.resolve()}. OK?", ast_namespace,
                       statement_using_statements, "expression", syntax_trees, BASE_EXPRESSION_CLASS_NAME)

if generate_statement:
    statement_namespace = str(statement_namespace).strip()

    if len(statement_namespace) <= 0:
        print("Error: statement tree namespace is empty", file=sys.stderr)
        exit(1)

    statement_using_statements = [
        "using System;",
        "using System.Collections.Generic;",
        "using System.Linq;",
        "using System.Text;",
        "using Giosue;",
        f"using {statement_namespace};"
    ]

    statement_trees: List[SyntaxTree] = [
        SyntaxTree(
            statement_namespace,
            "Expression",
            BASE_STATEMENT_CLASS_NAME,
            [
                Field("AST.Expression", "Expr", "expression")
            ]
        ),
        SyntaxTree(
            statement_namespace,
            "Var",
            BASE_STATEMENT_CLASS_NAME,
            [
                Field("Token", "Name", "name"),
                Field("AST.Expression", "Initializer", "initializer")
            ]
        ),
    ]

    statement_output_dir = Path(statement_output_dir)
    write_tree_to_file(statement_output_dir, f"Writing AST to {statement_output_dir.resolve()}. OK?",
                       statement_namespace, statement_using_statements, "statement",
                       statement_trees, BASE_STATEMENT_CLASS_NAME)


print("OK.", file=sys.stderr)
