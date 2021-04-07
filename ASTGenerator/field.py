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
        return f"public {self.type_name} {self.field_name} {{ get; }}"
