﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetApp
{
    public class BudgetService
    {
        private readonly IBudgetRepository _repo;

        public BudgetService(IBudgetRepository repo)
        {
            _repo = repo;
        }

        public decimal Query(DateTime startDate, DateTime endDate)
        {
            var budgets = this._repo.GetAll();
            if (startDate > endDate)
            {
                return 0;
            }

            decimal totalAmount = 0;
            if (IsSameMonth(startDate, endDate))
            {
                var currentMonth = startDate.ToString("yyyyMM");
                var budget = budgets.FirstOrDefault(x => x.YearMonth == currentMonth);
                if (budget != null)
                {
                    return budget.DailyAmount() * Period.DayCount(startDate, endDate);
                }
            }
            else
            {
                var currentDate = startDate;
                var allEndMonth = endDate.AddMonths(1);

                while (currentDate < allEndMonth)
                {
                    var currentBudget = FindBudget(currentDate, budgets);
                    if (currentBudget != null)
                    {
                        var effectiveDayCount = new Period(startDate, endDate).OverlappingDayCount(currentBudget);

                        totalAmount += currentBudget.DailyAmount() * effectiveDayCount;
                    }

                    currentDate = currentDate.AddMonths(1);
                }
            }

            return totalAmount;
        }

        private static Budget FindBudget(DateTime startDate, List<Budget> budgets)
        {
            var firstMonth = startDate.ToString("yyyyMM");
            var firstMonthBudget = FindBudget(budgets, firstMonth);
            return firstMonthBudget;
        }

        private static Budget FindBudget(List<Budget> budgets, string firstMonth)
        {
            var firstMonthBudget = budgets.FirstOrDefault(x => x.YearMonth == firstMonth);
            return firstMonthBudget;
        }

        private static bool IsSameMonth(DateTime startDate, DateTime endDate)
        {
            return startDate.ToString("yyyyMM") == endDate.ToString("yyyyMM");
        }

        private static decimal QuerySingleMonth(DateTime startDate, DateTime endDate, List<Budget> budgets)
        {
            var budget = budgets.FirstOrDefault(x => x.YearMonth == startDate.ToString("yyyyMM"));
            if (budget == null) return 0;

            return budget.DailyAmount() * (endDate.Day - startDate.Day + 1);
        }
    }
}