﻿using Antlr4.Runtime.Atn;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using TypeCobol.Compiler.Concurrency;
using TypeCobol.Compiler.Parser;
using TypeCobol.Compiler.Text;
using Antlr4.Runtime;
using TypeCobol.Compiler.Scanner;
using System.Diagnostics;

namespace TypeCobol.Compiler.AntlrUtils
{
    /// <summary>
    /// Useful information collected to evaluate the performance of Antlr parser
    /// </summary>
    public class AntlrPerformanceProfiler
    {
        public class ParsedFileInfo
        {
            public ParsedFileInfo(string textName, int parserRulesCount, int parserDecisionsCount)
            {
                Name = textName;
                RuleInvocations = new int[parserRulesCount];
                DecisionInfos = new DecisionInfo[parserDecisionsCount];
            }

            public string Name;
            public int LinesCount;
            public int TokensCount;
            public int CodeElementsCount;

            public long ParseTimeMs;
            public int[] RuleInvocations;

            public long DecisionTimeMs;
            public DecisionInfo[] DecisionInfos;

            public void AggregateOtherFileInfo(ParsedFileInfo otherFileInfo)
            {
                LinesCount += otherFileInfo.LinesCount;
                TokensCount += otherFileInfo.TokensCount;
                CodeElementsCount += otherFileInfo.CodeElementsCount;

                ParseTimeMs += otherFileInfo.ParseTimeMs;
                for (int i = 0; i < RuleInvocations.Length; i++)
                {
                    RuleInvocations[i] += otherFileInfo.RuleInvocations[i];
                }

                DecisionTimeMs += otherFileInfo.DecisionTimeMs;
                for (int i = 0; i < DecisionInfos.Length; i++)
                {
                    if (DecisionInfos[i] == null)
                    {
                        DecisionInfos[i] = new DecisionInfo(i);
                    }
                    DecisionInfos[i].invocations += otherFileInfo.DecisionInfos[i].invocations;
                    DecisionInfos[i].LL_ATNTransitions += otherFileInfo.DecisionInfos[i].LL_ATNTransitions;
                    DecisionInfos[i].LL_DFATransitions += otherFileInfo.DecisionInfos[i].LL_DFATransitions;
                    DecisionInfos[i].LL_Fallback += otherFileInfo.DecisionInfos[i].LL_Fallback;
                    DecisionInfos[i].LL_MaxLook += otherFileInfo.DecisionInfos[i].LL_MaxLook;
                    DecisionInfos[i].LL_MinLook += otherFileInfo.DecisionInfos[i].LL_MinLook;
                    DecisionInfos[i].LL_TotalLook += otherFileInfo.DecisionInfos[i].LL_TotalLook;
                    DecisionInfos[i].SLL_ATNTransitions += otherFileInfo.DecisionInfos[i].SLL_ATNTransitions;
                    DecisionInfos[i].SLL_DFATransitions += otherFileInfo.DecisionInfos[i].SLL_DFATransitions;
                    DecisionInfos[i].SLL_MaxLook += otherFileInfo.DecisionInfos[i].SLL_MaxLook;
                    DecisionInfos[i].SLL_MinLook += otherFileInfo.DecisionInfos[i].SLL_MinLook;
                    DecisionInfos[i].SLL_TotalLook += otherFileInfo.DecisionInfos[i].SLL_TotalLook;
                    DecisionInfos[i].timeInPrediction += otherFileInfo.DecisionInfos[i].timeInPrediction;
                }
            }
        }

        public AntlrPerformanceProfiler(Antlr4.Runtime.Parser parser)
        {
            parserRulesCount = parser.RuleNames.Length;
            parserRuleNames = parser.RuleNames;
            parserDecisionsCount = parser.Atn.DecisionToDfa.Length;

            ParsedFilesInfos = new List<ParsedFileInfo>();
            AggregatedPerfInfo = new ParsedFileInfo("TOTAL", parserRulesCount, parserDecisionsCount);
            RuleStacksCount = new Dictionary<string, int>();
        }

        private int parserRulesCount;
        private string[] parserRuleNames;
        private int parserDecisionsCount;

        public ParsedFileInfo CurrentFileInfo;
        public IList<ParsedFileInfo> ParsedFilesInfos;
        public ParsedFileInfo AggregatedPerfInfo;
        public IDictionary<string, int> RuleStacksCount;

        public void BeginParsingFile(TextSourceInfo textSourceInfo, ITokensLinesIterator tokensCountIterator)
        {            
            CurrentFileInfo = new ParsedFileInfo(textSourceInfo.Name, parserRulesCount, parserDecisionsCount);
            // Only for CodeElementsParser
            if (tokensCountIterator != null)
            {
                ITokensLine lastLine = null;
                Token token = null;
                while ((token = tokensCountIterator.NextToken()) != Token.END_OF_FILE)
                {
                    CurrentFileInfo.TokensCount++;
                    if (token.TokensLine != lastLine)
                    {
                        CurrentFileInfo.LinesCount++;
                        lastLine = token.TokensLine;
                    }
                }
            }
        }

        private Stopwatch chrono = new Stopwatch();

        internal void BeginParsingSection()
        {
            chrono.Start();
        }
        
        public void EnterParserRule(RuleContext ctx, int ruleIndex)
        {
            CurrentFileInfo.RuleInvocations[ruleIndex]++;
            string ruleStack = RuleStackBuilder.GetRuleStack(ctx, true);
            if(RuleStacksCount.ContainsKey(ruleStack))
            {
                RuleStacksCount[ruleStack]++;
            }
            else
            {
                RuleStacksCount[ruleStack] = 1;
            }
        }

        internal void EndParsingSection(int codeElementsCount)
        {
            chrono.Stop();
            CurrentFileInfo.ParseTimeMs += chrono.ElapsedMilliseconds;
            CurrentFileInfo.CodeElementsCount += codeElementsCount;
        }

        public void EndParsingFile(DecisionInfo[] decisionInfos, int decisionTimeMs)
        {
            CurrentFileInfo.DecisionInfos = decisionInfos;
            CurrentFileInfo.DecisionTimeMs = decisionTimeMs;
            ParsedFilesInfos.Add(CurrentFileInfo);
            AggregatedPerfInfo.AggregateOtherFileInfo(CurrentFileInfo);
        }

        public void  WriteInfoToResultFile(StreamWriter sw)
        {
            sw.WriteLine("---------------------");
            sw.WriteLine("Top rules invocations");
            sw.WriteLine("---------------------");
            sw.WriteLine("");

            IDictionary<string, int> ruleInvocations = new Dictionary<string, int>();
            for (int i = 0; i < AggregatedPerfInfo.RuleInvocations.Length; i++)
            {
                ruleInvocations.Add(parserRuleNames[i], AggregatedPerfInfo.RuleInvocations[i]);
            }
            foreach (var ruleStat in ruleInvocations.OrderByDescending(p => p.Value).Take(30))
            {
                sw.WriteLine(ruleStat.Value + "\t" + ruleStat.Key);
            }

            sw.WriteLine("");
            sw.WriteLine("---------------------");
            sw.WriteLine("Costly decisions info");
            sw.WriteLine("---------------------");
            sw.WriteLine("");

            foreach (var decisionInfo in AggregatedPerfInfo.DecisionInfos.Where(di => di.invocations > 0).OrderByDescending(di => di.timeInPrediction).Take(30))
            {
                sw.WriteLine(decisionInfo.decision + " : " + decisionInfo.invocations + " invocations, " + decisionInfo.timeInPrediction / 1000 / decisionInfo.invocations + " µs per decision, " + decisionInfo.timeInPrediction / 1000000 + " ms in prediction");
            }

            sw.WriteLine("");
            sw.WriteLine("---------------------");
            sw.WriteLine("Rules invocation info");
            sw.WriteLine("---------------------");

            int depth = 2;
            for (;;)
            {
                int count = 0;
                string prevstack = null;
                foreach (var ruleStackPair in RuleStacksCount.Where(p => p.Key.StartsWith(depth + ":")).OrderBy(p => LimitToDepth(p.Key, depth)).ThenByDescending(p => p.Value))
                {
                    string curstack = LimitToDepth(ruleStackPair.Key, depth);
                    if (prevstack != curstack)
                    {
                        sw.WriteLine();
                        sw.WriteLine(curstack);
                        prevstack = curstack;
                    }
                    sw.WriteLine("\t" + ruleStackPair.Value + "\t" + ruleStackPair.Key);
                    count++;
                }
                sw.WriteLine();
                sw.WriteLine();
                if (count == 0) break;
                depth++;
            }

            sw.WriteLine("");
            sw.WriteLine("-------------------");
            sw.WriteLine("Detailed file infos");
            sw.WriteLine("-------------------");
            sw.WriteLine("");

            foreach (var fileInfo in ParsedFilesInfos)
            {
                sw.WriteLine(fileInfo.Name);
                sw.WriteLine("\t" + fileInfo.LinesCount + " lines, " + fileInfo.TokensCount + " tokens, " + fileInfo.CodeElementsCount + " code elements");
                sw.WriteLine("\t" + fileInfo.ParseTimeMs + " ms parsing, " + fileInfo.DecisionTimeMs + " ms decision / " + fileInfo.RuleInvocations.Sum() + " rule invocations");
                sw.WriteLine("\t" + fileInfo.LinesCount * 1000 / fileInfo.ParseTimeMs + " lines/sec, " + fileInfo.TokensCount * 1000 / fileInfo.ParseTimeMs + " tokens/sec, " + fileInfo.CodeElementsCount * 1000 / fileInfo.ParseTimeMs + " code elements/sec");
            }
        }
        
        private static string LimitToDepth(string key, int depth)
        {
            int index = 0;
            int gtcount = 0;
            for (;;)
            {
                if (key[index] == '>') gtcount++;
                if (gtcount == depth - 1) break;
                index++;
            }

            return key.Substring(0, index);
        }
        
    }
}
