using System.Collections.Generic;

namespace Constants
{
	public static class Queries
	{
		public static List<QueryMeta> GetAllQueries()
		{
			return new List<QueryMeta>
			{
				new QueryMeta
				{
					Query = "Rotate 402-32-11-61-990-802-A by 35 degrees in Y axis",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"Rotate prev var1"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Rotate 402-32-11-61-990-802-A by 90 degrees in X axis",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"Rotate prev var1"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Scale up 402-32-11-61-990-802-A by 0.5",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"Scale up prev var1"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Scale down 402-32-11-61-990-802-A by 2",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"Scale down prev var1"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Show highlight [41], [46]",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"Highlight on prev"
					},
					Reply = "[41], [46]"
				},
				new QueryMeta
				{
					Query = "Remove highlight [41], [46]",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"Highlight off prev"
					},
					Reply = "[41], [46]"
				},
				new QueryMeta
				{
					Query = "Animate on 402-32-11-61-990-802-A",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"Animate on prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Animate off 402-32-11-61-990-802-A",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"Animate off prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Show left side of figure 402-32-11-61-990-802-A",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"ShowSide left prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Show top side of figure 402-32-11-61-990-802-A",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"ShowSide top prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Show bottom side of figure 402-32-11-61-990-802-A",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"ShowSide bottom prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Show back side of figure 402-32-11-61-990-802-A",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"ShowSide back prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Hide objects [41], [46] in this figure",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"Visibility off prev"
					},
					Reply = "[41], [46]"
				},
				new QueryMeta
				{
					Query = "Make visible objects [41], [46] in this figure",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"Visibility on prev"
					},
					Reply = "[41], [46]"
				},
				new QueryMeta
				{
					Query = "Show close look of objects [41], [46]",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"CloseLook prev"
					},
					Reply = "[41], [46]"
				},
				new QueryMeta
				{
					Query = "Show close look of objects [8], [43], [44], [46]",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"CloseLook prev"
					},
					Reply = "[8], [44]"
				},
				new QueryMeta
				{
					Query = "Show Side By Side view of items in figure 402-32-11-61-990-802-A",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"SideBySideLook prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Attach objects [41], [8]",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"Attach prev"
					},
					Reply = "[41], [8]"
				},
				new QueryMeta
				{
					Query = "Attach objects [45], [8]",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"Attach prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Attach objects [44], [46]",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"Attach prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Attach objects [46], [8]",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"Attach prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Attach objects [43], [46]",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"Attach prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Attach objects [42], [46]",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID object query",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Filter3DAttr name prev root",
						"Attach prev"
					},
					Reply = "402-32-11-61-990-802-A"
				},
				new QueryMeta
				{
					Query = "Reset 402-32-11-61-990-802-A",
					Programs = new[]
					{
						"ExtractNumbers query",
						"SaveVal2Var prev var1",
						"ExtractID figure query",
						"Unique prev",
						"SaveVal2Var prev var2",
						// "FilterType task root",
						// "FilterType subtask prev",
						// "FilterAttr subtaskId CurrentSubtaskId prev",
						// "Unique prev",
						// "QueryAttr figure prev",
						// "3DFilterAttr name prev",
						// "Unique prev",
						"Same var2 prev",
						"Filter3DAttr name prev root",
						"Unique prev",
						"Reset prev"
					},
					Reply = "402-32-11-61-990-802-A"
				}
			};
		}
	}
}