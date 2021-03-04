# defineAst(outputDir, "Expr", Arrays.asList(
#     "Binary   : Expr left, Token operator, Expr right",
#     "Grouping : Expr expression",
#     "Literal  : Object value",
#     "Unary    : Token operator, Expr right"
# ));

from typing import *
import argparse

INDENT = "    "


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


def indent(block: List[str], indent = INDENT):  # -> List[str]:
    return list(map(lambda f: indent + f, block))


class SyntaxTree:
    def __init__(self, namespace: str, name: str, base_class_name: str, fields: List[Field]):
        self.namespace: str = namespace
        self.name: str = name
        self.base_class_name: str = base_class_name
        self.fields: List[Field] = fields

    def get_constructor_parameters(self):
        return [field.get_constructor_parameter() for field in self.fields]
    
    def generate_fields(self):
        return [field.get_field_name() for field in self.fields]

    def get_field_initializers(self):
        return [field.get_initializer() for field in self.fields]
    
    def generate_constructor(self):
        parameters = ", ".join(self.get_constructor_parameters())
        initializers = indent(self.get_field_initializers())

        return [
            f"public {self.name}({parameters})",
            "{",
            *initializers,
            "}"
        ]

    def generate_class(self):
        fields = indent(self.generate_fields())
        constructor = indent(self.generate_constructor())

        return [
            f"public class {self.name} : {self.base_class_name}",
            "{",
            *fields,
            "\n",
            *constructor,
            "}"
        ]

    def generate_namespace(self):
        class_ = "\n".join(indent(self.generate_class()))

        return [
            f"namespace {self.namespace}",
            "{",
            f"{class_}",
            "}"
        ]

    def generate_tree(self):
        return "\n".join(self.generate_namespace())


# parser = argparse.ArgumentParser(description='Generate the ASTs for Giosue.')
# # parser.add_argument('integers', metavar='N', type=int, nargs='+',
# #                     help='an integer for the accumulator')
# # parser.add_argument('--sum', dest='accumulate', action='store_const',
# #                     const=sum, default=max,
# #                     help='sum the integers (default: find the max)')
# parser.add_argument('--generate-abstract-class', help="Generate the abstract base 'Expression' class.")

# args = parser.parse_args()


tr = SyntaxTree("namespc", "Name", "Base", [Field("string", "MyField", "myField")])
print(tr.generate_tree())
