﻿using System;
using System.Diagnostics;

namespace TypeCobol.Compiler.AntlrUtils
{
    public class PerfStatsForParserInvocation
    {
        public int AntlrParsingTime { get; set; }

        // Details of Antlr parsing time
        // Non zero only when Antlr performance profiling is activated
        public int DecisionTimeMs;
        public int RuleInvocationsCount;

        public int TreeBuildingTime { get; set; }

        private Stopwatch stopWatch = new Stopwatch();

        public void OnStartAntlrParsing()
        {
            stopWatch.Restart();
        }

        public void OnStopAntlrParsing(int decisionTimeMs, int ruleInvocationsCount)
        {
            stopWatch.Stop();
            AntlrParsingTime += (int)stopWatch.ElapsedMilliseconds;
            DecisionTimeMs += decisionTimeMs;
            RuleInvocationsCount += ruleInvocationsCount;
        }

        public void OnStartTreeBuilding()
        {
            stopWatch.Restart();
        }

        public void OnStopTreeBuilding()
        {
            stopWatch.Stop();
            TreeBuildingTime += (int)stopWatch.ElapsedMilliseconds;
        }

        internal void Add(PerfStatsForParserInvocation perfStats)
        {
            AntlrParsingTime += perfStats.AntlrParsingTime;
            DecisionTimeMs += perfStats.DecisionTimeMs;
            RuleInvocationsCount += perfStats.RuleInvocationsCount;
            TreeBuildingTime += TreeBuildingTime;
        }
    }
}
