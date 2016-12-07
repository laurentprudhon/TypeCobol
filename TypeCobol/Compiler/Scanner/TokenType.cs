﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.Compiler.Scanner
{
    // WARNING : both enumerations below (families / types) must stay in sync
    // WARNING : make sure to update the tables in TokenUtils if you add one more token family or one more token type

    public enum TokenFamily
    {
        //          0 : Error
        Invalid=0,
        //   1 ->   3 : Whitespace
        // p46: The separator comma and separator semicolon can
        // be used anywhere the separator space is used.
        Whitespace=1,
        //   4 ->   5 : Comments
        Comments=4,
        // 6 ->  10 : Separators - Syntax
        SyntaxSeparator=6,
        //  11 ->  15 : Special character word - Arithmetic operators
        ArithmeticOperator=11,
        //  16 ->  20 : Special character word - Relational operators
        RelationalOperator=16,
        //  21 ->  26 : Literals - Alphanumeric
        AlphanumericLiteral=21,
        //  27 ->  29 : Literals - Numeric 
        NumericLiteral=27,
        //  30 ->  32 : Literals - Syntax tokens
        SyntaxLiteral=30,
        //  33 ->  36 : Symbols
        Symbol=33,
        //  37 ->  55 : Keywords - Compiler directive starting tokens
        CompilerDirectiveStartingKeyword=37,
        //  56 ->  78 : Keywords - Code element starting tokens
        CodeElementStartingKeyword=56,
        //  79 ->  119: Keywords - Statement starting tokens
        StatementStartingKeyword=79,
        //  120 -> 140 : Keywords - Statement ending tokens
        StatementEndingKeyword=120,
        // 141 -> 171 : Keywords - Special registers
        SpecialRegisterKeyword=141,
        // 172 -> 185 : Keywords - Figurative constants
        FigurativeConstantKeyword=172,
        // 186 -> 187 : Keywords - Special object identifiers
        SpecialObjetIdentifierKeyword=186,
        // 188 -> 449 : Keywords - Syntax tokens
        SyntaxKeyword=188,
        // 450 -> 451 : Keywords - Cobol 2002
        Cobol2002Keyword = 450,
        // 452 -> 457 : Keywords - TypeCobol
        TypeCobolKeyword = 452,
        // 458 -> 460 : Compiler directives
        CompilerDirective = 458,
        // 461 -> 461 : Internal token groups - used by the preprocessor only
        InternalTokenGroup = 461
    }

    // INFO : the list below is generated from the file Documentation/Studies/CobolLexer.tokens.xls
    // WARNING : the list of tokens in CobolWords.g4 must stay in sync

    public enum TokenType
    {
        EndOfFile = -1,
        // Error
        InvalidToken = 0,
        // Separators - Whitespace
        SpaceSeparator = 1,
        CommaSeparator = 2,
        SemicolonSeparator = 3,
        // Comments
        FloatingComment = 4,
        CommentLine = 5,
        // Separators - Syntax
        PeriodSeparator = 6,
        ColonSeparator = 7,
        LeftParenthesisSeparator = 8,
        RightParenthesisSeparator = 9,
        PseudoTextDelimiter = 10,
        // Special character word - Arithmetic operators
        PlusOperator = 11,
        MinusOperator = 12,
        DivideOperator = 13,
        MultiplyOperator = 14,
        PowerOperator = 15,
        // Special character word - Relational operators
        LessThanOperator = 16,
        GreaterThanOperator = 17,
        LessThanOrEqualOperator = 18,
        GreaterThanOrEqualOperator = 19,
        EqualOperator = 20,
        // Literals - Alphanumeric
        AlphanumericLiteral = 21,
        HexadecimalAlphanumericLiteral = 22,
        NullTerminatedAlphanumericLiteral = 23,
        NationalLiteral = 24,
        HexadecimalNationalLiteral = 25,
        DBCSLiteral = 26,
        // Literals - Numeric
        IntegerLiteral = 27,
        DecimalLiteral = 28,
        FloatingPointLiteral = 29,
        // Literals - Syntax tokens
        PictureCharacterString = 30,
        CommentEntry = 31,
        ExecStatementText = 32,
        // Symbols
        IntrinsicFunctionName = 33,
        ExecTranslatorName = 34,
        PartialCobolWord = 35,
        UserDefinedWord = 36,
        // Keywords - Compiler directive starting tokens
        ASTERISK_CBL = 37,
        ASTERISK_CONTROL = 38,
        BASIS = 39,
        CBL = 40,
        COPY = 41,
        DELETE_CD = 42,
        EJECT = 43,
        ENTER = 44,
        EXEC_SQL_INCLUDE = 45,
        INSERT = 46,
        PROCESS = 47,
        READY = 48,
        RESET = 49,
        REPLACE = 50,
        SERVICE = 51,
        SKIP1 = 52,
        SKIP2 = 53,
        SKIP3 = 54,
        TITLE = 55,
        // Keywords - Code element starting tokens
        APPLY = 56,
        CONFIGURATION = 57,
        ELSE = 58,
        ENVIRONMENT = 59,
        FD = 60,
        FILE_CONTROL = 61,
        I_O_CONTROL = 62,
        ID = 63,
        IDENTIFICATION = 64,
        INPUT_OUTPUT = 65,
        LINKAGE = 66,
        LOCAL_STORAGE = 67,
        MULTIPLE = 68,
        OBJECT_COMPUTER = 69,
        REPOSITORY = 70,
        RERUN = 71,
        SAME = 72,
        SD = 73,
        SELECT = 74,
        SOURCE_COMPUTER = 75,
        SPECIAL_NAMES = 76,
        USE = 77,
        WORKING_STORAGE = 78,
        // Keywords - Statement starting tokens
        ACCEPT = 79,
        ADD = 80,
        ALTER = 81,
        CALL = 82,
        CANCEL = 83,
        CLOSE = 84,
        COMPUTE = 85,
        CONTINUE = 86,
        DELETE = 87,
        DISPLAY = 88,
        DIVIDE = 89,
        ENTRY = 90,
        EVALUATE = 91,
        EXEC = 92,
        EXECUTE = 93,
        EXIT = 94,
        GOBACK = 95,
        GO = 96,
        IF = 97,
        INITIALIZE = 98,
        INSPECT = 99,
        INVOKE = 100,
        MERGE = 101,
        MOVE = 102,
        MULTIPLY = 103,
        OPEN = 104,
        PERFORM = 105,
        READ = 106,
        RELEASE = 107,
        RETURN = 108,
        REWRITE = 109,
        SEARCH = 110,
        SET = 111,
        SORT = 112,
        START = 113,
        STOP = 114,
        STRING = 115,
        SUBTRACT = 116,
        UNSTRING = 117,
        WRITE = 118,
        XML = 119,
        // Keywords - Statement ending tokens
        END_ADD = 120,
        END_CALL = 121,
        END_COMPUTE = 122,
        END_DELETE = 123,
        END_DIVIDE = 124,
        END_EVALUATE = 125,
        END_EXEC = 126,
        END_IF = 127,
        END_INVOKE = 128,
        END_MULTIPLY = 129,
        END_PERFORM = 130,
        END_READ = 131,
        END_RETURN = 132,
        END_REWRITE = 133,
        END_SEARCH = 134,
        END_START = 135,
        END_STRING = 136,
        END_SUBTRACT = 137,
        END_UNSTRING = 138,
        END_WRITE = 139,
        END_XML = 140,
        // Keywords - Special registers
        ADDRESS = 141,
        DEBUG_CONTENTS = 142,
        DEBUG_ITEM = 143,
        DEBUG_LINE = 144,
        DEBUG_NAME = 145,
        DEBUG_SUB_1 = 146,
        DEBUG_SUB_2 = 147,
        DEBUG_SUB_3 = 148,
        JNIENVPTR = 149,
        LENGTH = 150,
        LINAGE_COUNTER = 151,
        RETURN_CODE = 152,
        SHIFT_IN = 153,
        SHIFT_OUT = 154,
        SORT_CONTROL = 155,
        SORT_CORE_SIZE = 156,
        SORT_FILE_SIZE = 157,
        SORT_MESSAGE = 158,
        SORT_MODE_SIZE = 159,
        SORT_RETURN = 160,
        TALLY = 161,
        WHEN_COMPILED = 162,
        XML_CODE = 163,
        XML_EVENT = 164,
        XML_INFORMATION = 165,
        XML_NAMESPACE = 166,
        XML_NAMESPACE_PREFIX = 167,
        XML_NNAMESPACE = 168,
        XML_NNAMESPACE_PREFIX = 169,
        XML_NTEXT = 170,
        XML_TEXT = 171,
        // Keywords - Figurative constants
        HIGH_VALUE = 172,
        HIGH_VALUES = 173,
        LOW_VALUE = 174,
        LOW_VALUES = 175,
        NULL = 176,
        NULLS = 177,
        QUOTE = 178,
        QUOTES = 179,
        SPACE = 180,
        SPACES = 181,
        ZERO = 182,
        ZEROES = 183,
        ZEROS = 184,
        SymbolicCharacter = 185,
        // Keywords - Special object identifiers
        SELF = 186,
        SUPER = 187,
        // Keywords - Syntax tokens
        ACCESS = 188,
        ADVANCING = 189,
        AFTER = 190,
        ALL = 191,
        ALPHABET = 192,
        ALPHABETIC = 193,
        ALPHABETIC_LOWER = 194,
        ALPHABETIC_UPPER = 195,
        ALPHANUMERIC = 196,
        ALPHANUMERIC_EDITED = 197,
        ALSO = 198,
        ALTERNATE = 199,
        AND = 200,
        ANY = 201,
        ARE = 202,
        AREA = 203,
        AREAS = 204,
        ASCENDING = 205,
        ASSIGN = 206,
        AT = 207,
        ATTRIBUTE = 208,
        ATTRIBUTES = 209,
        AUTHOR = 210,
        BEFORE = 211,
        BEGINNING = 212,
        BINARY = 213,
        BLANK = 214,
        BLOCK = 215,
        BOTTOM = 216,
        BY = 217,
        CHARACTER = 218,
        CHARACTERS = 219,
        CLASS = 220,
        CLASS_ID = 221,
        COBOL = 222,
        CODE = 223,
        CODE_SET = 224,
        COLLATING = 225,
        COM_REG = 226,
        COMMA = 227,
        COMMON = 228,
        COMP = 229,
        COMP_1 = 230,
        COMP_2 = 231,
        COMP_3 = 232,
        COMP_4 = 233,
        COMP_5 = 234,
        COMPUTATIONAL = 235,
        COMPUTATIONAL_1 = 236,
        COMPUTATIONAL_2 = 237,
        COMPUTATIONAL_3 = 238,
        COMPUTATIONAL_4 = 239,
        COMPUTATIONAL_5 = 240,
        CONTAINS = 241,
        CONTENT = 242,
        CONVERTING = 243,
        CORR = 244,
        CORRESPONDING = 245,
        COUNT = 246,
        CURRENCY = 247,
        DATA = 248,
        DATE = 249,
        DATE_COMPILED = 250,
        DATE_WRITTEN = 251,
        DAY = 252,
        DAY_OF_WEEK = 253,
        DBCS = 254,
        DEBUGGING = 255,
        DECIMAL_POINT = 256,
        DECLARATIVES = 257,
        DELIMITED = 258,
        DELIMITER = 259,
        DEPENDING = 260,
        DESCENDING = 261,
        DISPLAY_ARG = 262,
        DISPLAY_1 = 263,
        DIVISION = 264,
        DOWN = 265,
        DUPLICATES = 266,
        DYNAMIC = 267,
        EBCDIC = 268,
        EGCS = 269,
        ELEMENT = 270,
        ENCODING = 271,
        END = 272,
        END_OF_PAGE = 273,
        ENDING = 274,
        ENTRY_ARG = 275,
        EOP = 276,
        EQUAL = 277,
        ERROR = 278,
        EVERY = 279,
        EXCEPTION = 280,
        EXTEND = 281,
        EXTERNAL = 282,
        FACTORY = 283,
        FALSE = 284,
        FILE = 285,
        FILLER = 286,
        FIRST = 287,
        FOOTING = 288,
        FOR = 289,
        FROM = 290,
        FUNCTION = 291,
        FUNCTION_POINTER = 292,
        GENERATE = 293,
        GIVING = 294,
        GLOBAL = 295,
        GREATER = 296,
        GROUP_USAGE = 297,
        I_O = 298,
        IN = 299,
        INDEX = 300,
        INDEXED = 301,
        INHERITS = 302,
        INITIAL = 303,
        INPUT = 304,
        INSTALLATION = 305,
        INTO = 306,
        INVALID = 307,
        IS = 308,
        JUST = 309,
        JUSTIFIED = 310,
        KANJI = 311,
        KEY = 312,
        LABEL = 313,
        LEADING = 314,
        LEFT = 315,
        LESS = 316,
        LINAGE = 317,
        LINE = 318,
        LINES = 319,
        LOCK = 320,
        MEMORY = 321,
        METHOD = 322,
        METHOD_ID = 323,
        MODE = 324,
        MODULES = 325,
        MORE_LABELS = 326,
        NAME = 327,
        NAMESPACE = 328,
        NAMESPACE_PREFIX = 329,
        NATIONAL = 330,
        NATIONAL_EDITED = 331,
        NATIVE = 332,
        NEGATIVE = 333,
        NEW = 334,
        NEXT = 335,
        NO = 336,
        NONNUMERIC = 337,
        NOT = 338,
        NUMERIC = 339,
        NUMERIC_EDITED = 340,
        OBJECT = 341,
        OCCURS = 342,
        OF = 343,
        OFF = 344,
        OMITTED = 345,
        ON = 346,
        OPTIONAL = 347,
        OR = 348,
        ORDER = 349,
        ORGANIZATION = 350,
        OTHER = 351,
        OUTPUT = 352,
        OVERFLOW = 353,
        OVERRIDE = 354,
        PACKED_DECIMAL = 355,
        PADDING = 356,
        PAGE = 357,
        PARSE = 358,
        PASSWORD = 359,
        PIC = 360,
        PICTURE = 361,
        POINTER = 362,
        POSITION = 363,
        POSITIVE = 364,
        PROCEDURE = 365,
        PROCEDURE_POINTER = 366,
        PROCEDURES = 367,
        PROCEED = 368,
        PROCESSING = 369,
        PROGRAM = 370,
        PROGRAM_ID = 371,
        RANDOM = 372,
        RECORD = 373,
        RECORDING = 374,
        RECORDS = 375,
        RECURSIVE = 376,
        REDEFINES = 377,
        REEL = 378,
        REFERENCE = 379,
        REFERENCES = 380,
        RELATIVE = 381,
        RELOAD = 382,
        REMAINDER = 383,
        REMOVAL = 384,
        RENAMES = 385,
        REPLACING = 386,
        RESERVE = 387,
        RETURNING = 388,
        REVERSED = 389,
        REWIND = 390,
        RIGHT = 391,
        ROUNDED = 392,
        RUN = 393,
        SECTION = 394,
        SECURITY = 395,
        SEGMENT_LIMIT = 396,
        SENTENCE = 397,
        SEPARATE = 398,
        SEQUENCE = 399,
        SEQUENTIAL = 400,
        SIGN = 401,
        SIZE = 402,
        SORT_ARG = 403,
        SORT_MERGE = 404,
        SQL = 405,
        SQLIMS = 406,
        STANDARD = 407,
        STANDARD_1 = 408,
        STANDARD_2 = 409,
        STATUS = 410,
        SUPPRESS = 411,
        SYMBOL = 412,
        SYMBOLIC = 413,
        SYNC = 414,
        SYNCHRONIZED = 415,
        TALLYING = 416,
        TAPE = 417,
        TEST = 418,
        THAN = 419,
        THEN = 420,
        THROUGH = 421,
        THRU = 422,
        TIME = 423,
        TIMES = 424,
        TO = 425,
        TOP = 426,
        TRACE = 427,
        TRAILING = 428,
        TRUE = 429,
        TYPE = 430,
        UNBOUNDED = 431,
        UNIT = 432,
        UNTIL = 433,
        UP = 434,
        UPON = 435,
        USAGE = 436,
        USING = 437,
        VALIDATING = 438,
        VALUE = 439,
        VALUES = 440,
        VARYING = 441,
        WHEN = 442,
        WITH = 443,
        WORDS = 444,
        WRITE_ONLY = 445,
        XML_DECLARATION = 446,
        XML_SCHEMA = 447,
        YYYYDDD = 448,
        YYYYMMDD = 449,
        // Cobol 2002 tokens
        TYPEDEF = 450,
        STRONG = 451,
        // TypeCobol tokens
        DECLARE = 452,
        END_DECLARE = 453,
        UNSAFE = 454,
        PUBLIC = 455,
        PRIVATE = 456,
        INOUT = 457,
        // Group of tokens produced by the preprocessor
        // - compiler directives
        CompilerDirective = 458,
        CopyImportDirective = 459,
        ReplaceDirective = 460,
        // - internal token groups -> used by the preprocessor only
        ContinuationTokenGroup = 461
    }
}
