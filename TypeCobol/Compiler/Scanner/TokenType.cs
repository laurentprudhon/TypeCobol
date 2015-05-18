﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeCobol.Compiler.Scanner
{
    // WARNING : both enumerations (families / types) must stay in sync
    // WARNING : make sure to update the tables in TokenUtils if you add one more token family or one more token type

    public enum TokenFamily
    {
        //          0 : Error
        Invalid=0,
        //   1 ->   4 : Whitespace
        // p46: The separator comma and separator semicolon can
        // be used anywhere the separator space is used.
        Whitespace=1,
        //   5 ->   6 : Comments
        Comments=5,
        // 7 ->  11 : Separators - Syntax
        // -1 (EndOfFile)
        SyntaxSeparator=7,
        //  12 ->  16 : Special character word - Arithmetic operators
        ArithmeticOperator=12,
        //  17 ->  21 : Special character word - Relational operators
        RelationalOperator=17,
        //  22 ->  27 : Literals - Alphanumeric
        AlphanumericLiteral=22,
        //  28 ->  30 : Literals - Numeric 
        NumericLiteral=28,
        //  31 ->  33 : Literals - Syntax tokens
        SyntaxLiteral=31,
        //  34 ->  37 : Symbols
        Symbol=34,
        //  38 ->  56 : Keywords - Compiler directive starting tokens
        CompilerDirectiveStartingKeyword=38,
        //  57 ->  97 : Keywords - Statement starting tokens
        StatementStartingKeyword=57,
        //  98 -> 118 : Keywords - Statement ending tokens
        StatementEndingKeyword=98,
        // 119 -> 149 : Keywords - Special registers
        SpecialRegisterKeyword=119,
        // 150 -> 163 : Keywords - Figurative constants
        FigurativeConstantKeyword=150,
        // 164 -> 165 : Keywords - Special object identifiers
        SpecialObjetIdentifierKeyword=164,
        // 166 -> 450 : Keywords - Syntax tokens  
        SyntaxKeyword=166,
        // 451 -> 453 : Compiler directives
        CompilerDirective = 451,
        // 454 -> 454 : Internal token groups - used by the preprocessor only
        InternalTokenGroup = 454
    }

    public enum TokenType
    {
        // Error
        InvalidToken=0,
        // Separators - Whitespace
        SpaceSeparator=1,
        CommaSeparator=2,
        SemicolonSeparator=3,
        // Comments
        FloatingComment=5,
        CommentLine=6,
        // Separators - Syntax
        EndOfFile = -1,  // End of file => value -1 is expected by Antlr
        PeriodSeparator=7,
        ColonSeparator=8,
        LeftParenthesisSeparator=9,
        RightParenthesisSeparator=10,
        PseudoTextDelimiter = 11,
        // Special character word - Arithmetic operators
        PlusOperator=12,
        MinusOperator=13,
        DivideOperator=14,
        MultiplyOperator=15,
        PowerOperator=16,
        // Special character word - Relational operators
        LessThanOperator=17,
        GreaterThanOperator=18,
        LessThanOrEqualOperator=19,
        GreaterThanOrEqualOperator=20,
        EqualOperator=21,
        // Literals - Alphanumeric
        AlphanumericLiteral = 22,
        HexadecimalAlphanumericLiteral = 23,
        NullTerminatedAlphanumericLiteral = 24,
        NationalLiteral = 25,
        HexadecimalNationalLiteral = 26,
        DBCSLiteral = 27,
        // Literals - Numeric 
        IntegerLiteral = 28,
        DecimalLiteral = 29,
        FloatingPointLiteral = 30,
        // Literals - Syntax tokens
        PictureCharacterString = 31,
        CommentEntry = 32,
        ExecStatementText = 33,
        // Symbols
        FunctionName = 34,
        ExecTranslatorName = 35,
        PartialCobolWord = 36,
        UserDefinedWord = 37,
        // Keywords - Compiler directive starting tokens
        ASTERISK_CBL = 38,
        ASTERISK_CONTROL = 39,
        BASIS = 40,
        CBL = 41,
        COPY = 42,
        DELETE_CD = 43,
        EJECT = 44,
        ENTER = 45,
        EXEC_SQL_INCLUDE = 46,
        INSERT = 47,
        PROCESS = 48,
        READY = 49,
        RESET = 50,
        REPLACE = 51,
        SERVICE = 52,
        SKIP1 = 53,
        SKIP2 = 54,
        SKIP3 = 55,
        TITLE = 56,
        // Keywords - Statement starting tokens
        ACCEPT = 57,
        ADD = 58,
        ALTER = 59,
        CALL = 60,
        CANCEL = 61,
        CLOSE = 62,
        COMPUTE = 63,
        CONTINUE = 64,
        DELETE = 65,
        DISPLAY = 66,
        DIVIDE = 67,
        ENTRY = 68,
        EVALUATE = 69,
        EXEC = 70,
        EXECUTE = 71,
        EXIT = 72,
        GOBACK = 73,
        GO = 74,
        IF = 75,
        INITIALIZE = 76,
        INSPECT = 77,
        INVOKE = 78,
        MERGE = 79,
        MOVE = 80,
        MULTIPLY = 81,
        OPEN = 82,
        PERFORM = 83,
        READ = 84,
        RELEASE = 85,
        RETURN = 86,
        REWRITE = 87,
        SEARCH = 88,
        SET = 89,
        SORT = 90,
        START = 91,
        STOP = 92,
        STRING = 93,
        SUBTRACT = 94,
        UNSTRING = 95,
        WRITE = 96,
        XML = 97,
        // Keywords - Statement ending tokens
        END_ADD = 98,
        END_CALL = 99,
        END_COMPUTE = 100,
        END_DELETE = 101,
        END_DIVIDE = 102,
        END_EVALUATE = 103,
        END_EXEC = 104,
        END_IF = 105,
        END_INVOKE = 106,
        END_MULTIPLY = 107,
        END_PERFORM = 108,
        END_READ = 109,
        END_RETURN = 110,
        END_REWRITE = 111,
        END_SEARCH = 112,
        END_START = 113,
        END_STRING = 114,
        END_SUBTRACT = 115,
        END_UNSTRING = 116,
        END_WRITE = 117,
        END_XML = 118,
        // Keywords - Special registers
        ADDRESS = 119,
        DEBUG_CONTENTS = 120,
        DEBUG_ITEM = 121,
        DEBUG_LINE = 122,
        DEBUG_NAME = 123,
        DEBUG_SUB_1 = 124,
        DEBUG_SUB_2 = 125,
        DEBUG_SUB_3 = 126,
        JNIENVPTR = 127,
        LENGTH = 128,
        LINAGE_COUNTER = 129,
        RETURN_CODE = 130,
        SHIFT_IN = 131,
        SHIFT_OUT = 132,
        SORT_CONTROL = 133,
        SORT_CORE_SIZE = 134,
        SORT_FILE_SIZE = 135,
        SORT_MESSAGE = 136,
        SORT_MODE_SIZE = 137,
        SORT_RETURN = 138,
        TALLY = 139,
        WHEN_COMPILED = 140,
        XML_CODE = 141,
        XML_EVENT = 142,
        XML_INFORMATION = 143,
        XML_NAMESPACE = 144,
        XML_NAMESPACE_PREFIX = 145,
        XML_NNAMESPACE = 146,
        XML_NNAMESPACE_PREFIX = 147,
        XML_NTEXT = 148,
        XML_TEXT = 149,
        // Keywords - Figurative constants
        HIGH_VALUE = 150,
        HIGH_VALUES = 151,
        LOW_VALUE = 152,
        LOW_VALUES = 153,
        NULL = 154,
        NULLS = 155,
        QUOTE = 156,
        QUOTES = 157,
        SPACE = 158,
        SPACES = 159,
        ZERO = 160,
        ZEROES = 161,
        ZEROS = 162,
        SymbolicCharacter = 163,
        // Keywords - Special object identifiers
        SELF = 164,
        SUPER = 165,
        // Keywords - Syntax tokens
        ACCESS = 166,
        ADVANCING = 167,
        AFTER = 168,
        ALL = 169,
        ALPHABET = 170,
        ALPHABETIC = 171,
        ALPHABETIC_LOWER = 172,
        ALPHABETIC_UPPER = 173,
        ALPHANUMERIC = 174,
        ALPHANUMERIC_EDITED = 175,
        ALSO = 176,
        ALTERNATE = 177,
        AND = 178,
        ANY = 179,
        APPLY = 180,
        ARE = 181,
        AREA = 182,
        AREAS = 183,
        ASCENDING = 184,
        ASSIGN = 185,
        AT = 186,
        ATTRIBUTE = 187,
        ATTRIBUTES = 188,
        AUTHOR = 189,
        BEFORE = 190,
        BEGINNING = 191,
        BINARY = 192,
        BLANK = 193,
        BLOCK = 194,
        BOTTOM = 195,
        BY = 196,
        CHARACTER = 197,
        CHARACTERS = 198,
        CLASS = 199,
        CLASS_ID = 200,
        COBOL = 201,
        CODE = 202,
        CODE_SET = 203,
        COLLATING = 204,
        COM_REG = 205,
        COMMA = 206,
        COMMON = 207,
        COMP = 208,
        COMP_1 = 209,
        COMP_2 = 210,
        COMP_3 = 211,
        COMP_4 = 212,
        COMP_5 = 213,
        COMPUTATIONAL = 214,
        COMPUTATIONAL_1 = 215,
        COMPUTATIONAL_2 = 216,
        COMPUTATIONAL_3 = 217,
        COMPUTATIONAL_4 = 218,
        COMPUTATIONAL_5 = 219,
        CONFIGURATION = 220,
        CONTAINS = 221,
        CONTENT = 222,
        CONVERTING = 223,
        CORR = 224,
        CORRESPONDING = 225,
        COUNT = 226,
        CURRENCY = 227,
        DATA = 228,
        DATE = 229,
        DATE_COMPILED = 230,
        DATE_WRITTEN = 231,
        DAY = 232,
        DAY_OF_WEEK = 233,
        DBCS = 234,
        DEBUGGING = 235,
        DECIMAL_POINT = 236,
        DECLARATIVES = 237,
        DELIMITED = 238,
        DELIMITER = 239,
        DEPENDING = 240,
        DESCENDING = 241,
        DISPLAY_ARG = 242,
        DISPLAY_1 = 243,
        DIVISION = 244,
        DOWN = 245,
        DUPLICATES = 246,
        DYNAMIC = 247,
        EBCDIC = 248,
        EGCS = 249,
        ELEMENT = 250,
        ELSE = 251,
        ENCODING = 252,
        END = 253,
        END_OF_PAGE = 254,
        ENDING = 255,
        ENTRY_ARG = 256,
        ENVIRONMENT = 257,
        EOP = 258,
        EQUAL = 259,
        ERROR = 260,
        EVERY = 261,
        EXCEPTION = 262,
        EXTEND = 263,
        EXTERNAL = 264,
        FACTORY = 265,
        FALSE = 266,
        FD = 267,
        FILE = 268,
        FILE_CONTROL = 269,
        FILLER = 270,
        FIRST = 271,
        FOOTING = 272,
        FOR = 273,
        FROM = 274,
        FUNCTION = 275,
        FUNCTION_POINTER = 276,
        GENERATE = 277,
        GIVING = 278,
        GLOBAL = 279,
        GREATER = 280,
        GROUP_USAGE = 281,
        I_O = 282,
        I_O_CONTROL = 283,
        ID = 284,
        IDENTIFICATION = 285,
        IN = 286,
        INDEX = 287,
        INDEXED = 288,
        INHERITS = 289,
        INITIAL = 290,
        INPUT = 291,
        INPUT_OUTPUT = 292,
        INSTALLATION = 293,
        INTO = 294,
        INVALID = 295,
        IS = 296,
        JUST = 297,
        JUSTIFIED = 298,
        KANJI = 299,
        KEY = 300,
        LABEL = 301,
        LEADING = 302,
        LEFT = 303,
        LESS = 304,
        LINAGE = 305,
        LINE = 306,
        LINES = 307,
        LINKAGE = 308,
        LOCAL_STORAGE = 309,
        LOCK = 310,
        MEMORY = 311,
        METHOD = 312,
        METHOD_ID = 313,
        MODE = 314,
        MODULES = 315,
        MORE_LABELS = 316,
        MULTIPLE = 317,
        NAME = 318,
        NAMESPACE = 319,
        NAMESPACE_PREFIX = 320,
        NATIONAL = 321,
        NATIONAL_EDITED = 322,
        NATIVE = 323,
        NEGATIVE = 324,
        NEW = 325,
        NEXT = 326,
        NO = 327,
        NONNUMERIC = 328,
        NOT = 329,
        NUMERIC = 330,
        NUMERIC_EDITED = 331,
        OBJECT = 332,
        OBJECT_COMPUTER = 333,
        OCCURS = 334,
        OF = 335,
        OFF = 336,
        OMITTED = 337,
        ON = 338,
        OPTIONAL = 339,
        OR = 340,
        ORDER = 341,
        ORGANIZATION = 342,
        OTHER = 343,
        OUTPUT = 344,
        OVERFLOW = 345,
        OVERRIDE = 346,
        PACKED_DECIMAL = 347,
        PADDING = 348,
        PAGE = 349,
        PARSE = 350,
        PASSWORD = 351,
        PIC = 352,
        PICTURE = 353,
        POINTER = 354,
        POSITION = 355,
        POSITIVE = 356,
        PROCEDURE = 357,
        PROCEDURE_POINTER = 358,
        PROCEDURES = 359,
        PROCEED = 360,
        PROCESSING = 361,
        PROGRAM = 362,
        PROGRAM_ID = 363,
        RANDOM = 364,
        RECORD = 365,
        RECORDING = 366,
        RECORDS = 367,
        RECURSIVE = 368,
        REDEFINES = 369,
        REEL = 370,
        REFERENCE = 371,
        REFERENCES = 372,
        RELATIVE = 373,
        RELOAD = 374,
        REMAINDER = 375,
        REMOVAL = 376,
        RENAMES = 377,
        REPLACING = 378,
        REPOSITORY = 379,
        RERUN = 380,
        RESERVE = 381,
        RETURNING = 382,
        REVERSED = 383,
        REWIND = 384,
        RIGHT = 385,
        ROUNDED = 386,
        RUN = 387,
        SAME = 388,
        SD = 389,
        SECTION = 390,
        SECURITY = 391,
        SEGMENT_LIMIT = 392,
        SELECT = 393,
        SENTENCE = 394,
        SEPARATE = 395,
        SEQUENCE = 396,
        SEQUENTIAL = 397,
        SIGN = 398,
        SIZE = 399,
        SORT_ARG = 400,
        SORT_MERGE = 401,
        SOURCE_COMPUTER = 402,
        SPECIAL_NAMES = 403,
        SQL = 404,
        SQLIMS = 405,
        STANDARD = 406,
        STANDARD_1 = 407,
        STANDARD_2 = 408,
        STATUS = 409,
        SUPPRESS = 410,
        SYMBOL = 411,
        SYMBOLIC = 412,
        SYNC = 413,
        SYNCHRONIZED = 414,
        TALLYING = 415,
        TAPE = 416,
        TEST = 417,
        THAN = 418,
        THEN = 419,
        THROUGH = 420,
        THRU = 421,
        TIME = 422,
        TIMES = 423,
        TO = 424,
        TOP = 425,
        TRACE = 426,
        TRAILING = 427,
        TRUE = 428,
        TYPE = 429,
        UNBOUNDED = 430,
        UNIT = 431,
        UNTIL = 432,
        UP = 433,
        UPON = 434,
        USAGE = 435,
        USE = 436,
        USING = 437,
        VALIDATING = 438,
        VALUE = 439,
        VALUES = 440,
        VARYING = 441,
        WHEN = 442,
        WITH = 443,
        WORDS = 444,
        WORKING_STORAGE = 445,
        WRITE_ONLY = 446,
        XML_DECLARATION = 447,
        XML_SCHEMA = 448,
        YYYYDDD = 449,
        YYYYMMDD = 450,
        // Group of tokens produced by the preprocessor
        // - compiler directives
        CompilerDirective = 451,
        CopyImportDirective = 452,
        ReplaceDirective = 453,
        // - internal token groups -> used by the preprocessor only
        ContinuationTokenGroup = 454
    }
}
