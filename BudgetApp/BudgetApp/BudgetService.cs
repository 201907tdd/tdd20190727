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

            if (startDate.ToString("yyyyMM") == endDate.ToString("yyyyMM"))
            {
                return QuerySingleMonth(startDate, endDate, budgets);
            }
            else
            {
                string searchMonth = "";
                var firstmonth = budgets.FirstOrDefault(x => x.YearMonth == startDate.ToString("yyyyMM")).Amount /
                    DateTime.DaysInMonth(startDate.Year, startDate.Month) *
                    (DateTime.DaysInMonth(startDate.Year, startDate.Month) - startDate.Day + 1);
                var secondmonth = budgets.FirstOrDefault(x => x.YearMonth == endDate.ToString("yyyyMM")).Amount /
                    DateTime.DaysInMonth(endDate.Year, endDate.Month) * (endDate.Day);

                var totalAmount = firstmonth + secondmonth;
                var allStartMonth = new DateTime(startDate.Year, startDate.Month, 1).AddMonths(1);
                var allEndMonth = new DateTime(endDate.Year, endDate.Month, 1);
                while (allEndMonth > allStartMonth)
                {
                    searchMonth = allStartMonth.ToString("yyyyMM");
                    if (budgets.Any(x => x.YearMonth == searchMonth))
                        totalAmount += budgets.FirstOrDefault(x => x.YearMonth == searchMonth).Amount;

                    allStartMonth = allStartMonth.AddMonths(1);
                }

                return totalAmount;
            }
        }

        private static decimal QuerySingleMonth(DateTime startDate, DateTime endDate, List<Budget> budgets)
        {
            var searchMonth = startDate.ToString("yyyyMM");
            if (budgets.All(x => x.YearMonth != searchMonth))
            {
                return 0;
            }

            if (endDate.Day == DateTime.DaysInMonth(startDate.Year, startDate.Month) && startDate.Day == 1)
            {
                return budgets.FirstOrDefault(x => x.YearMonth == searchMonth).Amount;
            }
            else if (startDate.Day == endDate.Day)
            {
                return budgets.FirstOrDefault(x => x.YearMonth == searchMonth).Amount /
                    DateTime.DaysInMonth(startDate.Year, startDate.Month);
            }
            else
            {
                return budgets.FirstOrDefault(x => x.YearMonth == searchMonth).Amount /
                    DateTime.DaysInMonth(startDate.Year, startDate.Month) * (endDate.Day - startDate.Day + 1);
            }
        }
    }
}