// Glen De Cauwsemaecker licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using L20nCore.Internal;
using L20nCore.Utils;

namespace L20nCore
{
	namespace Objects
	{
		/// <summary>
		/// <see cref="L20nCore.Objects.Entity"/> is the most import class in a way.
		/// Instances of this class are the actual root objects that will be retrieved and evaluated
		/// for translation, and objects of other L20n types will be evaluated only as a result of being
		/// linked to an instance of this class.
		/// </summary>
		public sealed class Entity : L20nObject
		{	
			/// <summary>
			/// Gets a value indicating whether this instance is private.
			/// </summary>
			/// <value><c>true</c> if this instance is private; otherwise, <c>false</c>.</value>
			public bool IsPrivate
			{
				get { return m_IsPrivate; }
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Entity"/> class.
			/// </summary>
			public Entity(L20nObject index, bool is_private, L20nObject value, L20nObject attributes)
			{
				m_Index = index;
				m_Value = value;
				m_Attributes = attributes;
				m_IsPrivate = is_private;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="L20nCore.Objects.Entity"/> class,
			/// where the value is a <see cref="L20nCore.Objects.HashValue"/> created based on the given external value.
			/// The index/attributes will be <c>null</c> and this instace will be public.
			/// </summary>
			public Entity(External.IHashValue value)
			{
				var info = External.InfoCollector.Pool.GetObject();

				value.Collect(info);

				m_Index = null;
				m_Value = info.Collect();
				m_Attributes = null;
				m_IsPrivate = false;

				info.Clear();
				External.InfoCollector.Pool.ReturnObject(ref info);
			}

			/// <summary>
			/// Can't be optimized and will return this instead.
			/// </summary>
			public override L20nObject Optimize()
			{
				return this;
			}

			/// <summary>
			/// Will return <c>null</c> in case this is a root entity and this instance is private,
			/// or in case something else went wrong.
			/// Otherwise it will evaluate the value, optionally using the first given <c>argv</c> parameter,
			/// which will always result in a <see cref="L20nCore.Objects.StringOutput"/> value in case of success.
			/// </summary>
			public override L20nObject Eval(LocaleContext ctx, params L20nObject[] argv)
			{
				// if the first given parameter is of L20n type dummy, we'll check if we're not private.
				// if not we can continue and will simply re-call this method with the
				// dummy object removed from the given parameters.
				if (argv.Length != 0 && (argv [0] as Dummy) != null)
				{
					if (m_IsPrivate)
					{
						Logger.Warning("entity is marked as private and cannot be accessed from C#");
						return null;
					}

					if (argv.Length > 1)
					{
						var arguments = new L20nObject[argv.Length - 1];
						for (int i = 0; i < arguments.Length; ++i)
							arguments [i] = argv [i + 1];
						return this.Eval(ctx, arguments);
					} else
					{
						return this.Eval(ctx);
					}
				}
	
				// if index is given and no extern parameters have been given,
				// we'll evaluate the index and will use that to evaluate the final output value.
				// this also assumes that in case the index is given,
				// that the value is a HashValue rather than a StringValue,
				// which is something the parser of this type enforces anyhow.
				if (m_Index != null && argv.Length == 0)
				{
					var index = m_Index.Eval(ctx);
					if (index == null)
					{
						Logger.Warning("Entity: index couldn't be evaluated");
						return index;
					}

					// could be one simple identifier, in which case we can evaluate
					// the HashValue of this instance given the identifier as a parameter
					var identifier = index as Identifier;
					if (identifier != null)
					{
						var result = m_Value.Eval(ctx, identifier);
						if (result == null)
						{
							Logger.Warning("<Entity>: <Identifier>-index got evaluated to null");
							return null;
						}

						return result.Eval(ctx);
					}

					// otherwise it should be a PropertyExpression,
					// in which case we evaluate that expression given this instance as the first parameter.
					var property = index as PropertyExpression;
					if (property != null)
					{
						var result = property.Eval(ctx, this);
						if (result == null)
						{
							Logger.Warning("<Entity>: <PropertyExpression>-index got evaluated to null");
							return null;
						}

						return result.Eval(ctx);
					}

					Logger.Warning("couldn't evaluate entity as index was expexted to be a <property_expression>");
					return null;
				}

				// in all other cases we simply evaluate the wrapped value,
				// passing on the given external parameters (e.g. an index)
				return m_Value.Eval(ctx, argv);
			}

			public L20nObject GetAttribute(LocaleContext ctx, Identifier identifier)
			{
				return m_Attributes.Eval(ctx, identifier);
			}
			
			private readonly L20nObject m_Index;
			private readonly L20nObject m_Value;
			private readonly L20nObject m_Attributes;
			private readonly bool m_IsPrivate;
		}
	}
}
