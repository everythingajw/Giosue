from typing import List
from typing import NoReturn
from pathlib import Path
from sys import stderr

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


def exists_and_is_directory_or_exit(p: Path) -> NoReturn:
    if not p.exists():
        print("Error: the output directory does not exist.", file=sys.stderr)
        exit(1)
    elif not p.is_dir():
        print("Error: the output directory is not a directory.", file=sys.stderr)
        exit(2)


def prompt_yes_no(prompt: str, default_no: bool = True) -> bool:
    """
    Prompts the user with a yes/no prompt.
    :param prompt: The question to ask the user
    :param default_no: Indicates if "no" is the default response
    :return: True if the user responded "yes", false if they responded "no"
    """

    response = None
    yes_no_suffix = "(y/N)" if default_no else "(Y/n)"
    while response != "" and response not in ALL_YES_NO_RESPONSES:
        print(f"{prompt} {yes_no_suffix} ", file=stderr, end="")
        response = input().strip().lower()

    if response == "":
        # return False if default_no else True
        return not default_no

    # The response is one of the acceptable yes/no responses, so
    # it's safe to do this check here.
    return response in YES_RESPONSES
