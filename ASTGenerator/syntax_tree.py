# Giosue language interpreter
# The interpreter for the Giosue programming language.
# Copyright (C) 2021  Anthony Webster
# 
# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2 of the License, or
# (at your option) any later version.
# 
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
# 
# You should have received a copy of the GNU General Public License along
# with this program; if not, write to the Free Software Foundation, Inc.,
# 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

from common import *
from typing import List
from field import Field


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
