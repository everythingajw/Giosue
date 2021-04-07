from typing import List

INDENT = "    "
BASE_EXPRESSION_CLASS_NAME = "Expression"
GENERIC_PARAMETER = "T"
VISITOR_INTERFACE_NAME = f"IVisitor<{GENERIC_PARAMETER}>"
YES_RESPONSES = ("yes", "y")
NO_RESPONSES = ("no", "n")
ALL_YES_NO_RESPONSES = (*YES_RESPONSES, *NO_RESPONSES)


def indent(block: List[str], spaces=INDENT) -> List[str]:
    return list(map(lambda line: spaces + line, block))


def add_namespace(block, name):
    return [
        f"namespace {name}",
        "{",
        *indent(block),
        "}"
    ]
