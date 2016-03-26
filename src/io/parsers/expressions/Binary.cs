/**
 * This source file is part of the Commercial L20n Unity Plugin.
 * 
 * Copyright (c) 2016 Glen De Cauwsemaecker (contact@glendc.com)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;

using L20nCore.Exceptions;

namespace L20nCore
{
	namespace IO
	{
		namespace Parsers
		{
			namespace Expressions
			{
				public class Binary
				{
					public static AST.INode Parse(CharStream stream)
					{
						var startingPos = stream.Position;
						
						try {
							var first = Unary.Parse(stream);
							string raw;
							if(stream.ReadReg(@"\s*(\=\=|\!\=|\<\=|\>\=|\<|\>|\+|\-|\*|\/|\%)", out raw)) {
								var chain = new Chain(stream, first, raw);
								return chain.Build();
							}
							else {
								return first;
							}
						}
						catch(Exception e) {
							string msg = String.Format(
								"something went wrong parsing an <binary_expression> starting at {0}",
								stream.ComputeDetailedPosition(startingPos));
							throw new Exceptions.ParseException(msg, e);
						}
					}

					/// <summary>
					/// A class to ensure that a chain of binary expressions
					/// is evaluated in the correct order.
					/// 
					/// That means that it needs to respect the mathematical importance
					/// in such a way that we can allow the user the rely on these
					/// implicit rules that are conventional.
					/// </summary>
					private class Chain
					{
						// All nodes will be collected here
						private List<Node> m_Nodes;

						// The constructor collects all the nodes
						public Chain(CharStream stream, AST.INode node, string op)
						{
							m_Nodes = new List<Node>(2);

							do {
								m_Nodes.Add(new Node(node, op));
								WhiteSpace.Parse(stream, true);
								node = Unary.Parse(stream);
							} while(stream.ReadReg (@"\s*(\=\=|\!\=|\<\=|\>\=|\<|\>|\+|\-|\*|\/|\%)", out op));
						
							m_Nodes.Add(new Node(node));
						}

						// We star building recursively at the first position
						public AST.INode Build()
						{
							return BuildRecursive(0, m_Nodes[0].Value);
						}

						private AST.INode BuildRecursive(int i, AST.INode previous)
						{
							// we should never be at the last position at this point
							// as this would mean that we have an operator as the last symbol
							// in a binary expression, which wouldn't make sense
							// and so if this ever happens we probably have a bug in our parsing logic
							if (i >= m_Nodes.Count - 1)
								throw new ParseException("something terrible went wrong during parsing");

							// cache the first 2 nodes
							var first = m_Nodes[i];
							var second = m_Nodes[i + 1];
						
							// if the second node has an operator,
							// it means that we have to check if it has
							// presedence over the current pair
							if (second.Op.IsSet) {
								var firstOp = (Operation) first.Op.Unwrap();
								var secondOp = (Operation) second.Op.Unwrap();

								// in this case the next operator has presedence
								// over the current one, which means we have to evaluate that one first
								// this is akin to drawing the implicit parenthesis
								if(firstOp.Rank < secondOp.Rank) {
									var binOp = BuildRecursive(i + 1, second.Value);
									return new AST.BinaryOperation(
										previous, binOp, firstOp.Value);
								}
								// otherwise we can simply evaluate as we would from left to right
								else if(firstOp.Rank >= secondOp.Rank) {
									var binOp = new AST.BinaryOperation(
										previous, second.Value, firstOp.Value);
									return BuildRecursive(i + 1, binOp);
								}
							}

							// if we have no operators left on the right, we can simply
							// evaluate as we reached the end.
							return new AST.BinaryOperation (
								previous, second.Value,
								((Operation) first.Op.Unwrap()).Value);
						}

						///<summary>
						/// A private class that contains an AST node
						/// and an optional operator. It's used to easily pass around
						/// these intermediate values in a compact collection
						///</summary>
						private class Node
						{
							public Utils.Option<Operation> Op {
								get;
								private set;
							}
							
							public AST.INode Value {
								get;
								private set;
							}
							
							public Node(AST.INode value, string op = null)
							{
								var operation =
									op == null ? null : new Operation(op.Trim());
								Op = new Utils.Option<Operation>(operation);
								Value = value;
							}
						}

						///<summary>
						/// This class wraps around the real BinaryOperation enumerator.
						/// It calculates the correct enumeration value based on a raw string
						/// and defines also the operator-rank at that point.
						/// This rank defines wether or not it has presedence over another.
						///</summary>
						private class Operation
						{
							public AST.BinaryOperation.Operation Value {
								get;
								private set;
							}
							
							public int Rank {
								get;
								private set;
							}
							
							public Operation(string raw)
							{
								switch(raw.Trim()) {
								case "==":
									Value = AST.BinaryOperation.Operation.IsEqual;
									Rank = 0; break;
									
								case "!=":
									Value = AST.BinaryOperation.Operation.IsNotEqual;
									Rank = 0; break;
									
								case "<":
									Value = AST.BinaryOperation.Operation.LessThan;
									Rank = 0; break;
									
								case ">": 
									Value = AST.BinaryOperation.Operation.GreaterThan;
									Rank = 0; break;
									
								case "<=":
									Value = AST.BinaryOperation.Operation.LessThanOrEqual;
									Rank = 0; break;
									
								case ">=":
									Value = AST.BinaryOperation.Operation.GreaterThanOrEqual;
									Rank = 0; break;
									
								case "*":
									Value = AST.BinaryOperation.Operation.Multiply;
									Rank = 2; break;
									
								case "/":
									Value = AST.BinaryOperation.Operation.Divide;
									Rank = 2; break;
									
								case "%":
									Value = AST.BinaryOperation.Operation.Modulo;
									Rank = 2; break;
									
								case "+":
									Value = AST.BinaryOperation.Operation.Add;
									Rank = 1; break;
									
								case "-":
									Value = AST.BinaryOperation.Operation.Subtract;
									Rank = 1; break;
									
								default:
									throw new ParseException(
										String.Format("{0} is not a valid <binary> operation", raw));
								}
							}
						}
					}
				}
			}
		}
	}
}
