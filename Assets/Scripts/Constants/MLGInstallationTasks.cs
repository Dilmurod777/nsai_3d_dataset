using System.Collections.Generic;

namespace Constants
{
	public static class MLGInstallationTasks
	{
		public static readonly Dictionary<string, Dictionary<string, dynamic>> Tasks = new Dictionary<string, Dictionary<string, dynamic>>
		{
			{
				"TASK 32-11-61-400-801", new Dictionary<string, dynamic>
				{
					{"title", "Main Landing Gear Upper Side Strut Installation"},
					{
						"subtasks", new Dictionary<string, Dictionary<string, dynamic>>
						{
							{
								"SUBTASK 32-11-61-420-001", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-420-001"},
									{"figureImage", "figure_upper_side_c"},
									{"cameraLocationsRange", new List<int> {0, 5}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Attach", Components = new List<string> {"[3]", "[15]"}}, // 0
											new Action {Name = "Attach", Components = new List<string> {"[22]", "[3]"}}, // 1
											new Action {Name = "Attach", Components = new List<string> {"[21]", "[20]", "[22]"}}, // 2
											new Action {Name = "Attach", Components = new List<string> {"[16]", "[22]"}}, // 3
											new Action {Name = "Attach", Components = new List<string> {"[17]", "[18]", "[16]"}}, // 4
											new Action {Name = "Attach", Components = new List<string> {"[19]", "[16]"}}, // 5
										}
									}
								}
							},
							{
								"SUBTASK 32-11-61-420-002", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-420-002"},
									{"figureImage", "figure_upper_side_b"},
									{"cameraLocationsRange", new List<int> {6, 12}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Attach", Components = new List<string> {"[8]", "[3]"}}, // 6
											new Action {Name = "Attach", Components = new List<string> {"[7]", "[8]"}}, // 7
											new Action {Name = "Attach", Components = new List<string> {"[9]", "[7]"}}, // 8
											new Action {Name = "Attach", Components = new List<string> {"[10]", "[7]"}}, // 9
											new Action {Name = "Attach", Components = new List<string> {"[11]", "[7]"}}, // 10
											new Action {Name = "Attach", Components = new List<string> {"[12]", "[14]", "[11]"}}, // 11
											new Action {Name = "Attach", Components = new List<string> {"[13]", "[11]"}}, // 12
										}
									}
								}
							},
							{
								"SUBTASK 32-11-61-420-003", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-420-003"},
									{"figureImage", "figure_upper_side_a"},
									{"cameraLocationsRange", new List<int> {13, 16}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Attach", Components = new List<string> {"[1]", "[3]"}}, // 13
											new Action {Name = "Attach", Components = new List<string> {"[2]", "[1]"}}, // 14
											new Action {Name = "Attach", Components = new List<string> {"[4]", "[5]", "[2]"}}, // 15
											new Action {Name = "Attach", Components = new List<string> {"[6]", "[2]"}} // 16
										}
									}
								}
							}
						}
					},
				}
			},
			{
				"TASK 32-11-61-400-802", new Dictionary<string, dynamic>
				{
					{"title", "Main Landing Gear Lower Side Strut Installation"},
					{
						"subtasks", new Dictionary<string, Dictionary<string, dynamic>>
						{
							// {
							// 	"SUBTASK 32-11-61-420-014", new Dictionary<string, dynamic>
							// 	{
							// 		{"title", "SUBTASK 32-11-61-420-014"},
							// 		{"figureImage", "figure_upper_side_b"},
							// 		{"cameraLocationsRange", new List<int> {6, 12}},
							// 		{
							// 			"instructions", new List<Action>
							// 			{
							// 				new Action {Name = "Attach", Components = new List<string> {"[8]", "[3]"}}, // 6
							// 				new Action {Name = "Attach", Components = new List<string> {"[7]", "[8]"}}, // 7
							// 				new Action {Name = "Attach", Components = new List<string> {"[9]", "[7]"}}, // 8
							// 				new Action {Name = "Attach", Components = new List<string> {"[10]", "[7]"}}, // 9
							// 				new Action {Name = "Attach", Components = new List<string> {"[11]", "[7]"}}, // 10
							// 				new Action {Name = "Attach", Components = new List<string> {"[12]", "[14]", "[11]"}}, // 11
							// 				new Action {Name = "Attach", Components = new List<string> {"[13]", "[11]"}}, // 12
							// 			}
							// 		}
							// 	}
							// },
							{
								"SUBTASK 32-11-61-420-006", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-420-006"},
									{"figureImage", "figure_lower_side_b"},
									{"cameraLocationsRange", new List<int> {17, 22}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Attach", Components = new List<string> {"[8]", "[51]"}}, // 17
											new Action {Name = "Attach", Components = new List<string> {"[52]", "[8]"}}, // 18
											new Action {Name = "Attach", Components = new List<string> {"[53]", "[54]", "[52]"}}, // 19
											new Action {Name = "Attach", Components = new List<string> {"[47]", "[52]"}}, // 20
											new Action {Name = "Attach", Components = new List<string> {"[48]", "[50]", "[47]"}}, // 21
											new Action {Name = "Attach", Components = new List<string> {"[49]", "[47]"}}, // 22
										}
									}
								}
							},
							{
								"SUBTASK 32-11-61-420-007", new Dictionary<string, dynamic>
								{
									{"title", "SUBTASK 32-11-61-420-007"},
									{"figureImage", "figure_lower_side_a"},
									{"cameraLocationsRange", new List<int> {23, 27}},
									{
										"instructions", new List<Action>
										{
											new Action {Name = "Attach", Components = new List<string> {"[41]", "[8]"}}, // 23
											new Action {Name = "Attach", Components = new List<string> {"[45]", "[41]"}}, // 24
											new Action {Name = "Attach", Components = new List<string> {"[46]", "[41]"}}, // 25
											new Action {Name = "Attach", Components = new List<string> {"[44]", "[46]"}}, // 26
											new Action {Name = "Attach", Components = new List<string> {"[43]", "[42]", "[46]"}} // 27
										}
									}
								}
							},
						}
					}
				}
			},
			// {
			// 	"TASK 32-11-61-400-803", new Dictionary<string, dynamic>
			// 	{
			// 		{"title", "Main Landing Gear Side Strut Installation"},
			// 		{
			// 			"subtasks", new Dictionary<string, Dictionary<string, dynamic>>
			// 			{
			// 				{
			// 					"SUBTASK 32-11-61-420-008", new Dictionary<string, dynamic>
			// 					{
			// 						{"title", "SUBTASK 32-11-61-420-008"},
			// 						{"figureImage", "figure_upper_side_c"},
			// 						{"cameraLocationsRange", new List<int> {0, 5}},
			// 						{
			// 							"instructions", new List<Action>
			// 							{
			// 								new Action {Name = "Attach", Components = new List<string> {"[3]", "[15]"}}, // 0
			// 								new Action {Name = "Attach", Components = new List<string> {"[22]", "[3]"}}, // 1
			// 								new Action {Name = "Attach", Components = new List<string> {"[21]", "[20]", "[22]"}}, // 2
			// 								new Action {Name = "Attach", Components = new List<string> {"[16]", "[22]"}}, // 3
			// 								new Action {Name = "Attach", Components = new List<string> {"[17]", "[18]", "[16]"}}, // 4
			// 								new Action {Name = "Attach", Components = new List<string> {"[19]", "[16]"}}, // 5
			// 							}
			// 						}
			// 					}
			// 				},
			// 				{
			// 					"SUBTASK 32-11-61-420-009", new Dictionary<string, dynamic>
			// 					{
			// 						{"title", "SUBTASK 32-11-61-420-009"},
			// 						{"figureImage", "figure_lower_side_b"},
			// 						{"cameraLocationsRange", new List<int> {17, 22}},
			// 						{
			// 							"instructions", new List<Action>
			// 							{
			// 								new Action {Name = "Attach", Components = new List<string> {"[8]", "[51]"}}, // 17
			// 								new Action {Name = "Attach", Components = new List<string> {"[52]", "[8]"}}, // 18
			// 								new Action {Name = "Attach", Components = new List<string> {"[53]", "[54]", "[52]"}}, // 19
			// 								new Action {Name = "Attach", Components = new List<string> {"[47]", "[52]"}}, // 20
			// 								new Action {Name = "Attach", Components = new List<string> {"[48]", "[50]", "[47]"}}, // 21
			// 								new Action {Name = "Attach", Components = new List<string> {"[49]", "[47]"}}, // 22
			// 							}
			// 						}
			// 					}
			// 				},
			// 				{
			// 					"SUBTASK 32-11-61-420-010", new Dictionary<string, dynamic>
			// 					{
			// 						{"title", "SUBTASK 32-11-61-420-010"},
			// 						{"figureImage", "figure_lower_side_a"},
			// 						{"cameraLocationsRange", new List<int> {23, 27}},
			// 						{
			// 							"instructions", new List<Action>
			// 							{
			// 								new Action {Name = "Attach", Components = new List<string> {"[41]", "[8]"}}, // 23
			// 								new Action {Name = "Attach", Components = new List<string> {"[45]", "[41]"}}, // 24
			// 								new Action {Name = "Attach", Components = new List<string> {"[46]", "[41]"}}, // 25
			// 								new Action {Name = "Attach", Components = new List<string> {"[44]", "[46]"}}, // 26
			// 								new Action {Name = "Attach", Components = new List<string> {"[43]", "[42]", "[46]"}} // 27
			// 							}
			// 						}
			// 					}
			// 				},
			// 				{
			// 					"SUBTASK 32-11-61-420-011", new Dictionary<string, dynamic>
			// 					{
			// 						{"title", "SUBTASK 32-11-61-420-011"},
			// 						{"figureImage", "figure_upper_side_a"},
			// 						{"cameraLocationsRange", new List<int> {13, 16}},
			// 						{
			// 							"instructions", new List<Action>
			// 							{
			// 								new Action {Name = "Attach", Components = new List<string> {"[1]", "[3]"}}, // 13
			// 								new Action {Name = "Attach", Components = new List<string> {"[2]", "[1]"}}, // 14
			// 								new Action {Name = "Attach", Components = new List<string> {"[4]", "[5]", "[2]"}}, // 15
			// 								new Action {Name = "Attach", Components = new List<string> {"[6]", "[2]"}} // 16
			// 							}
			// 						}
			// 					}
			// 				}
			// 			}
			// 		}
			// 	}
			// }
		};
	}
}