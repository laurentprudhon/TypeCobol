﻿using System.Collections.Generic;
using TypeCobol.Compiler.AntlrUtils;
using TypeCobol.Compiler.CodeElements;
using TypeCobol.Compiler.CodeElements.Expressions;
using TypeCobol.Compiler.Parser.Generated;
using System.Linq;

namespace TypeCobol.Compiler.Parser
{
    class LogicalExpressionBuilder
    {

        public LogicalExpression createCondition(CobolCodeElementsParser.ConditionalExpressionContext context)
        {
            if (context.simpleCondition() != null)
                return createCondition(context.simpleCondition());
            if (context.complexCondition() != null)
                return createCondition(context.complexCondition());
            return new Empty();
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.ComplexConditionContext context)
        {
            if (context.andCondition() != null)
            {
                var conditions = context.andCondition();
                if (conditions.Count > 0)
                {
                    LogicalExpression left = createCondition(conditions[0]);
                    if (conditions.Count > 1)
                    {
                        LogicalExpression right = createCondition(conditions[1]);
                        return new OR(left, right);
                    }
                    return left;
                }
            }
            return new Empty();
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.AndConditionContext context)
        {
            if (context.notCondition() != null)
            {
                var conditions = context.notCondition();
                if (conditions.Count > 0)
                {
                    LogicalExpression left = createCondition(conditions[0]);
                    if (conditions.Count > 1)
                    {
                        LogicalExpression right = createCondition(conditions[1]);
                        return new AND(left, right);
                    }
                    return left;
                }
            }
            return new Empty();
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.NotConditionContext context)
        {
            if (context.conditionBase() != null)
            {
                if (context.NOT() != null)
                {
                    return new NOT(createCondition(context.conditionBase()));
                }
                else
                {
                    return createCondition(context.conditionBase());
                }
            }
            return new Empty();
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.ConditionBaseContext context)
        {
            if (context.simpleCondition() != null)
                return createCondition(context.simpleCondition());
            if (context.complexCondition() != null)
                return createCondition(context.complexCondition());
            return new Empty();
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.SimpleConditionContext context)
        {
            if (context.classCondition() != null)
                return createCondition(context.classCondition());
            if (context.conditionNameCondition() != null)
                return createCondition(context.conditionNameCondition());
            if (context.relationCondition() != null)
                return createCondition(context.relationCondition());
            if (context.signCondition() != null)
                return createCondition(context.signCondition());
            if (context.switchStatusCondition() != null)
                return createCondition(context.switchStatusCondition());
            return new Empty();
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.ClassConditionContext context)
        {
            Symbol type = null;
            if (context.charsetClassName() != null) type = new CharsetClassName(ParseTreeUtils.GetFirstToken(context.charsetClassName()));
            if (context.NUMERIC() != null) type = new ClassName(ParseTreeUtils.GetFirstToken(context.NUMERIC()));
            if (context.ALPHABETIC() != null) type = new ClassName(ParseTreeUtils.GetFirstToken(context.ALPHABETIC()));
            if (context.ALPHABETIC_LOWER() != null) type = new ClassName(ParseTreeUtils.GetFirstToken(context.ALPHABETIC_LOWER()));
            if (context.ALPHABETIC_UPPER() != null) type = new ClassName(ParseTreeUtils.GetFirstToken(context.ALPHABETIC_UPPER()));
            if (context.DBCS() != null) type = new ClassName(ParseTreeUtils.GetFirstToken(context.DBCS()));
            if (context.KANJI() != null) type = new ClassName(ParseTreeUtils.GetFirstToken(context.KANJI()));
            LogicalExpression condition = new ClassCondition(new Identifier(context.identifier()), type);
            if (context.NOT() != null) condition = new NOT(condition);
            return condition;
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.ConditionNameConditionContext context)
        {
            if (context.conditionNameReference() == null) return new Empty();
            return createCondition(context.conditionNameReference().qualifiedConditionName());
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.QualifiedConditionNameContext qualified)
        {
            if (qualified == null) return new Empty();
            // we can take first, as conditionName is a UserDefinedWord
            var condition = new Condition(ParseTreeUtils.GetFirstToken(qualified.conditionName()));
            condition.inof.AddDataNames(qualified.inOrOfDataName());
            condition.inof.AddFileName(qualified.inOrOfFileName());
            // TODO add subscripts
            return condition;
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.RelationConditionContext context)
        {
            if (context.generalRelationCondition() != null)
                return createCondition(context.generalRelationCondition());
            if (context.dataPointerRelationCondition() != null)
                return createCondition(context.dataPointerRelationCondition());
            if (context.programPointerRelationCondition() != null)
                return createCondition(context.programPointerRelationCondition());
            if (context.objectReferenceRelationCondition() != null)
                return createCondition(context.objectReferenceRelationCondition());
            return new Empty();
        }

        private char createOperator(CobolCodeElementsParser.RelationalOperatorContext context)
        {
            var simple = context.simpleRelation();
            if (simple != null)
            {
                if (simple.GreaterThanOrEqualOperator() != null) return '≥';
                if (simple.LessThanOrEqualOperator() != null) return '≤';
                if (simple.OR() == null) ;//TODO diagnostic about missing word
                if (simple.EQUAL() == null) ;//TODO diagnostic about missing word
                if (simple.GREATER() != null) return '≥';
                if (simple.LESS() != null) return '≤';
            }
            bool inverted = context.NOT() != null;
            var strict = context.strictRelation();
            if (strict != null) {
                if (!inverted)
                {
                    if (strict.GreaterThanOperator() != null) return '>';
                    if (strict.LessThanOperator() != null) return '<';
                    if (strict.GREATER() != null) return '>';
                    if (strict.LESS() != null) return '<';
                    if (strict.EqualOperator() != null) return '=';
                    if (strict.EQUAL() != null) return '=';
                }
                else
                {
                    if (strict.GreaterThanOperator() != null) return '≤';
                    if (strict.LessThanOperator() != null) return '≥';
                    if (strict.GREATER() != null) return '≤';
                    if (strict.LESS() != null) return '≥';
                    if (strict.EqualOperator() != null) return '!';
                    if (strict.EQUAL() != null) return '!';
                }
            }
            return '?';
        }

        private Expression createOperand(CobolCodeElementsParser.OperandContext context)
        {
            if (context.identifier() != null) return new Identifier(context.identifier());
            if (context.literal() != null) return new Literal(context.literal());
            System.Console.WriteLine("TODO: IMPLEMENT NON-IDENTIFIER NON-LITERAL OPERANDS "+(context.identifier() != null) + " " + (context.literal() != null) + " " + (context.intrinsicFunction() != null) + " " + (context.arithmeticExpression() != null) + " " + (context.indexName() != null));
            return new Empty();
        }

        private char createOperator(CobolCodeElementsParser.AbbreviatedRelationContext context)
        {
            if (context.AND() != null) return '&';
            if (context.OR() != null) return '|';
            return '?';
        }

        private LogicOperation createAbbreviatedRelation(LogicOperation relation, CobolCodeElementsParser.AbbreviatedRelationContext[] relations)
        {
            if (relations.Length == 0) return relation;

            LogicOperation left = relation;
            LogicOperation right = null;
            char rightoperator = '?';
            for (int c = 0; c < relations.Length; c++)
            {
                var current = createRelation(relation, relations[c]);
                if (right == null)
                {
                    right = current;
                    rightoperator = createOperator(relations[c]);
                }
                else // relations[c].operator = AND
                {
                    right = LogicOperation.Create(right, createOperator(relations[c]), current);
                }
                if (c == relations.Length -1)
                {
                    left = LogicOperation.Create(left, rightoperator, right);
                    right = null;
                    break;
                }
                else
                if (relations[c +1].OR() != null)
                {
                    left = LogicOperation.Create(left, rightoperator, right);
                    right = null;
                }
            }
            return left;
        }

        private LogicOperation createRelation(LogicOperation relation, CobolCodeElementsParser.AbbreviatedRelationContext context)
        {
            Expression right = null;
            if (context.operand() != null)
            {
                right = createOperand(context.operand());
            }
            LogicOperation result = null;
            if (context.relationalOperator() != null)
            {
                var op = createOperator(context.relationalOperator());
                result = new Relation(relation.left, op, right);
            }
            else
            {
                result = new Relation(relation.left, relation.op, right);
            }
            return result;
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.GeneralRelationConditionContext context)
        {
            LogicOperation relation = null;
            char op = createOperator(context.relationalOperator());
            var operands = context.operand();
            if (operands != null && operands.Count > 0)
            {
                Expression left = createOperand(operands[0]);
                if (operands.Count > 1)
                {
                    Expression right = createOperand(operands[1]);
                    relation = new Relation(left, op, right);
                }
                else
                {
                    LogicOperation logical = left as LogicOperation;
                    if (logical != null)
                    {
                        relation = logical;
                    }
                    else
                    {
                        System.Console.WriteLine("TODO: Illogical expression \""+left+"\" sole part of a LogicalExpression !");
                        throw new System.NotImplementedException("Illogical expression \"" + left + "\" sole part of a LogicalExpression");
                    }
                }
            }
            var relations = context.abbreviatedRelation();
            if (relations != null && relations.Count > 0)
            {
                return createAbbreviatedRelation(relation, relations.ToArray());
            }
            return relation;
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.DataPointerRelationConditionContext context)
        {
            if (context.dataPointer() != null && context.dataPointer().Count > 0)
            {
                var first = context.dataPointer().ElementAt(0);
                Expression left = createOperand(context.dataPointer().ElementAt(0));
                if (first.ADDRESS() != null) left = new Pointer((Identifier)left);

                char op = createOperator(context.relationConditionEquality());

                Expression right = null;
                if (context.dataPointer().Count > 1)
                {
                    var second = context.dataPointer().ElementAt(1);
                    right = createOperand(second);
                    if (second.ADDRESS() != null) right = new Pointer((Identifier)right);
                }
                return LogicOperation.Create(left, op, right);
            }
            return new Empty();
        }

        private char createOperator(CobolCodeElementsParser.RelationConditionEqualityContext context)
        {
            if (context == null) return '?';
            if (context.NOT() != null) return '!';
            if (context.EqualOperator() != null || context.EQUAL() != null) return '=';
            return '?';
        }

        private Expression createOperand(CobolCodeElementsParser.DataPointerContext context)
        {
            if (context.NULL() != null || context.NULLS() != null) return new Null();
            if (context.identifier() != null) return new Identifier(context.identifier());
            return new Empty();
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.ProgramPointerRelationConditionContext context)
        {
            if (context.procedureOrFunctionPointer() != null && context.procedureOrFunctionPointer().Count > 0)
            {
                Expression left = createOperand(context.procedureOrFunctionPointer().ElementAt(0));

                char op = createOperator(context.relationConditionEquality());

                Expression right = null;
                if (context.procedureOrFunctionPointer().Count > 1)
                {
                    right = createOperand(context.procedureOrFunctionPointer().ElementAt(1));
                }
                return LogicOperation.Create(left, op, right);
            }
            return new Empty();
        }

        private Expression createOperand(CobolCodeElementsParser.ProcedureOrFunctionPointerContext context)
        {
            if (context.identifier() != null) return new Identifier(context.identifier());
            if (context.NULL() != null || context.NULLS() != null) return new Null();
            return new Empty();
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.ObjectReferenceRelationConditionContext context)
        {
            if (context.objectReference() != null && context.objectReference().Count > 0)
            {
                Expression left = createOperand(context.objectReference().ElementAt(0));

                char op = createOperator(context.relationConditionEquality());

                Expression right = null;
                if (context.objectReference().Count > 1)
                {
                    right = createOperand(context.objectReference().ElementAt(1));
                }
                return LogicOperation.Create(left, op, right);
            }
            return new Empty();
        }

        private Expression createOperand(CobolCodeElementsParser.ObjectReferenceContext context)
        {
            if (context.identifier() != null) return new Identifier(context.identifier());
            if (context.NULL() != null || context.NULLS() != null) return new Null();
            if (context.SELF() != null) return new Self();
            return new Empty();
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.SignConditionContext context)
        {
            System.Console.WriteLine("TODO: IMPLEMENT SIGN CONDITIONS");
            return new Empty();
        }

        private LogicalExpression createCondition(CobolCodeElementsParser.SwitchStatusConditionContext context)
        {
            return createCondition(context.qualifiedConditionName());
        }
    }
}