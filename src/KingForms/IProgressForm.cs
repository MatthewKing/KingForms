﻿namespace KingForms;

public interface IProgressForm
{
    void ReportProgressText(string progressText);
    void ReportProgressPercent(double progressPercent);
}