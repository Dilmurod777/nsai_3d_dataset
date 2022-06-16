using System.Collections.Generic;

namespace Constants
{
	public static class MLGRemovalTasks
	{
		public static readonly Dictionary<string, Dictionary<string, dynamic>> Tasks = new Dictionary<string, Dictionary<string, dynamic>>
		{
			{
				"TASK 32-11-61-000-801", new Dictionary<string, dynamic>
				{
					{"title", "Main Landing Gear Upper Side Strut Removal"},
					{
						"subtasks", new Dictionary<string, Dictionary<string, dynamic>>
						{
							{
								"SUBTASK 32-11-61-020-002", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-020-002"},
									{"figureImage", "figure_upper_side_a"},
									{"cameraLocationsRange", new List<int> {0, 3}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Detach", Components = new List<string> {"[6]", "[2]"}}, // 0
											new Action {Name = "Detach", Components = new List<string> {"[5]", "[4]", "[2]"}}, // 1
											new Action {Name = "Detach", Components = new List<string> {"[2]", "[1]"}}, // 2
											new Action {Name = "Detach", Components = new List<string> {"[1]", "[3]"}}, // 3
										}
									}
								}
							},
							{
								"SUBTASK 32-11-61-020-003", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-020-003"},
									{"figureImage", "figure_upper_side_b"},
									{"cameraLocationsRange", new List<int> {4, 8}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Detach", Components = new List<string> {"[13]", "[14]", "[12]", "[11]"}}, // 4
											new Action {Name = "Detach", Components = new List<string> {"[11]", "[7]"}}, // 5
											new Action {Name = "Detach", Components = new List<string> {"[9]", "[7]"}}, // 6
											new Action {Name = "Detach", Components = new List<string> {"[10]", "[7]"}}, // 7
											new Action {Name = "Detach", Components = new List<string> {"[7]", "[8]"}}, // 8
										}
									}
								}
							},
							{
								"SUBTASK 32-11-61-020-004", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-020-004"},
									{"figureImage", "figure_upper_side_c"},
									{"cameraLocationsRange", new List<int> {9, 12}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Detach", Components = new List<string> {"[19]", "[18]", "[17]", "[16]"}}, // 9
											new Action {Name = "Detach", Components = new List<string> {"[16]", "[22]"}}, // 10
											new Action {Name = "Detach", Components = new List<string> {"[20]", "[21]", "[22]"}}, // 11
											new Action {Name = "Detach", Components = new List<string> {"[22]", "[3]"}}, // 12
										}
									}
								}
							},
						}
					},
				}
			},
			{
				"TASK 32-11-61-000-802", new Dictionary<string, dynamic>
				{
					{"title", "Main Landing Gear Lower Side Strut Removal"},
					{
						"subtasks", new Dictionary<string, Dictionary<string, dynamic>>
						{
							{
								"SUBTASK 32-11-61-020-007", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-420-007"},
									{"figureImage", "figure_lower_side_a"},
									{"cameraLocationsRange", new List<int> {13, 16}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Detach", Components = new List<string> {"[42]", "[43]", "[46]"}}, // 13
											new Action {Name = "Detach", Components = new List<string> {"[46]", "[41]"}}, // 14
											new Action {Name = "Detach", Components = new List<string> {"[44]", "[45]", "[41]"}}, // 15
											new Action {Name = "Detach", Components = new List<string> {"[41]", "[8]"}}, // 16
										}
									}
								}
							},
							{
								"SUBTASK 32-11-61-020-008", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-020-008"},
									{"figureImage", "figure_lower_side_b"},
									{"cameraLocationsRange", new List<int> {17, 20}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Detach", Components = new List<string> {"[49]", "[50]", "[48]", "[47]"}}, // 17
											new Action {Name = "Detach", Components = new List<string> {"[47]", "[52]"}}, // 18
											new Action {Name = "Detach", Components = new List<string> {"[54]", "[53]", "[52]"}}, // 19
											new Action {Name = "Detach", Components = new List<string> {"[52]", "[8]"}}, // 20
										}
									}
								}
							},
							{
								"SUBTASK 32-11-61-020-009", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-020-009"},
									{"figureImage", "figure_lower_side_c"},
									{"cameraLocationsRange", new List<int> {21, 22}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Detach", Components = new List<string> {"[8]", "[51]"}}, // 21
											new Action {Name = "Detach", Components = new List<string> {"[3]", "[15]"}}, // 22
										}
									}
								}
							},
						}
					}
				}
			},
			// {
			// 	"TASK 32-11-61-000-803", new Dictionary<string, dynamic>
			// 	{
			// 		{"title", "Main Landing Gear Side Strut Removal"},
			// 		{
			// 			"subtasks", new Dictionary<string, Dictionary<string, dynamic>>
			// 			{
			// 				{
			// 					"SUBTASK 32-11-61-020-011", new Dictionary<string, dynamic>
			// 					{
			// 						{"title", "SUBTASK 32-11-61-020-011"},
			// 						{"figureImage", "figure_upper_side_a"},
			// 						{"cameraLocationsRange", new List<int> {0, 3}},
			// 						{
			// 							"instructions", new List<Action>
			// 							{
			// 								new Action {Name = "Detach", Components = new List<string> {"[6]", "[2]"}}, // 0
			// 								new Action {Name = "Detach", Components = new List<string> {"[5]", "[4]", "[2]"}}, // 1
			// 								new Action {Name = "Detach", Components = new List<string> {"[2]", "[1]"}}, // 2
			// 								new Action {Name = "Detach", Components = new List<string> {"[1]", "[3]"}}, // 3
			// 							}
			// 						}
			// 					}
			// 				},	
			// 				{
			// 					"SUBTASK 32-11-61-020-012", new Dictionary<string, dynamic>
			// 					{
			// 						{"title", "SUBTASK 32-11-61-420-012"},
			// 						{"figureImage", "figure_lower_side_a"},
			// 						{"cameraLocationsRange", new List<int> {13, 15}},
			// 						{
			// 							"instructions", new List<Action>
			// 							{
			// 								new Action {Name = "Detach", Components = new List<string> {"[42]", "[43]", "[46]"}}, // 13
			// 								new Action {Name = "Detach", Components = new List<string> {"[46]", "[41]"}}, // 14
			// 								new Action {Name = "Detach", Components = new List<string> {"[41]", "[8]"}}, // 15
			// 							}
			// 						}
			// 					}
			// 				},
			// 				{
			// 					"SUBTASK 32-11-61-020-013", new Dictionary<string, dynamic>
			// 					{
			// 						{"title", "SUBTASK 32-11-61-020-013"},
			// 						{"figureImage", "figure_lower_side_b"},
			// 						{"cameraLocationsRange", new List<int> {15, 19}},
			// 						{
			// 							"instructions", new List<Action>
			// 							{
			// 								new Action {Name = "Detach", Components = new List<string> {"[49]", "[50]", "[48]", "[47]"}}, // 15
			// 								new Action {Name = "Detach", Components = new List<string> {"[47]", "[52]"}}, // 16
			// 								new Action {Name = "Detach", Components = new List<string> {"[54]", "[53]", "[52]"}}, // 17
			// 								new Action {Name = "Detach", Components = new List<string> {"[52]", "[8]"}}, // 18
			// 							}
			// 						}
			// 					}
			// 				},
			// 				{
			// 					"SUBTASK 32-11-61-020-014", new Dictionary<string, dynamic>
			// 					{
			// 						{"title", "SUBTASK 32-11-61-020-014"},
			// 						{"figureImage", "figure_upper_side_c"},
			// 						{"cameraLocationsRange", new List<int> {8, 12}},
			// 						{
			// 							"instructions", new List<Action>
			// 							{
			// 								new Action {Name = "Detach", Components = new List<string> {"[19]", "[18]", "[17]", "[16]"}}, // 8
			// 								new Action {Name = "Detach", Components = new List<string> {"[16]", "[22]"}}, // 9
			// 								new Action {Name = "Detach", Components = new List<string> {"[20]", "[21]", "[22]"}}, // 10
			// 								new Action {Name = "Detach", Components = new List<string> {"[22]", "[3]"}}, // 11
			// 								new Action {Name = "Detach", Components = new List<string> {"[3]", "[15]"}}, // 12
			// 								new Action {Name = "Detach", Components = new List<string> {"[8]", "[51]"}} // 12
			// 							}
			// 						}
			// 					}
			// 				},
			// 			}
			// 		}
			// 	}
			// }
		};
	}
}