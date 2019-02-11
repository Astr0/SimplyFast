using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SimplyFast.Collections;
using SimplyFast.Reflection;

namespace SimplyFast.Expressions.Internal
{
    internal class DebugStringVisitor : ExpressionVisitor
    {
        private const int IdentMultiplier = 2;
        private readonly Dictionary<LabelTarget, int> _labelIds = new Dictionary<LabelTarget, int>();
        private readonly Dictionary<ParameterExpression, int> _paramIds = new Dictionary<ParameterExpression, int>();
        private readonly StringBuilder _sb = new StringBuilder();
        private string _ident = "";

        private void OutType(Type type)
        {
            Out(type.FriendlyName());
        }

        private void Out(string s)
        {
            _sb.Append(s);
        }

        private void OutIdent()
        {
            Out(_ident);
        }

        private void Indent(int inc)
        {
            var newIdent = _ident.Length + inc*IdentMultiplier;
            if (newIdent < 0)
                newIdent = 0;
            _ident = new string(' ', newIdent);
        }

        private void NewLine()
        {
            _sb.AppendLine();
            OutIdent();
        }

        private void Out(char c)
        {
            _sb.Append(c);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.ArrayIndex)
            {
                Visit(node.Left);
                Out("[");
                Visit(node.Right);
                Out("]");
            }
            else
            {
                if (IsChecked(node.NodeType))
                    Out("checked");
                Out("(");
                Visit(node.Left);
                Out(' ');
                Out(BinaryOperatorToString(node));
                Out(' ');
                Visit(node.Right);
                Out(")");
            }
            return node;
        }

        private static bool IsChecked(ExpressionType node)
        {
            return node == ExpressionType.AddAssignChecked
                   || node == ExpressionType.AddChecked
                   || node == ExpressionType.ConvertChecked
                   || node == ExpressionType.MultiplyAssignChecked
                   || node == ExpressionType.MultiplyChecked
                   || node == ExpressionType.NegateChecked
                   || node == ExpressionType.SubtractAssignChecked
                   || node == ExpressionType.SubtractChecked;
        }

        private static string BinaryOperatorToString(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.And:
                    return "&";
                case ExpressionType.AndAlso:
                    return "&&";
                case ExpressionType.Coalesce:
                    return "??";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Equal:
                    return "==";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.Or:
                    return "|";
                case ExpressionType.OrElse:
                    return "||";
                case ExpressionType.Power:
                    return "**";
                case ExpressionType.RightShift:
                    return ">>";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Assign:
                    return "=";
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked:
                    return "+=";
                case ExpressionType.AndAssign:
                    return "&=";
                case ExpressionType.DivideAssign:
                    return "/=";
                case ExpressionType.ExclusiveOrAssign:
                    return "^=";
                case ExpressionType.LeftShiftAssign:
                    return "<<=";
                case ExpressionType.ModuloAssign:
                    return "%=";
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked:
                    return "*=";
                case ExpressionType.OrAssign:
                    return "|=";
                case ExpressionType.PowerAssign:
                    return "**=";
                case ExpressionType.RightShiftAssign:
                    return ">>=";
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                    return "-=";
                default:
                    throw new InvalidOperationException();
            }
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.PreIncrementAssign:
                    Out("++");
                    Visit(node.Operand);
                    break;
                case ExpressionType.PreDecrementAssign:
                    Out("--");
                    Visit(node.Operand);
                    break;
                case ExpressionType.OnesComplement:
                    Out("~(");
                    Visit(node.Operand);
                    break;
                case ExpressionType.Increment:
                    Out('(');
                    Visit(node.Operand);
                    Out(" + 1)");
                    break;
                case ExpressionType.Throw:
                    Out("throw (");
                    Visit(node.Operand);
                    Out(')');
                    break;
                case ExpressionType.Unbox:
                    Out("Unbox");
                    goto case ExpressionType.TypeAs;
                case ExpressionType.Convert:
                    Out("Convert");
                    goto case ExpressionType.TypeAs;
                case ExpressionType.ConvertChecked:
                    Out("ConvertChecked");
                    goto case ExpressionType.TypeAs;
                case ExpressionType.TypeAs:
                    Out('(');
                    Visit(node.Operand);
                    Out(" As ");
                    OutType(node.Type);
                    Out(')');
                    break;
                case ExpressionType.Decrement:
                    Out('(');
                    Visit(node.Operand);
                    Out(" - 1)");
                    break;
                case ExpressionType.Negate:
                    Out('-');
                    Visit(node.Operand);
                    break;
                case ExpressionType.NegateChecked:
                    Out("checked(-");
                    Visit(node.Operand);
                    Out(')');
                    break;
                case ExpressionType.UnaryPlus:
                    Out('+');
                    Visit(node.Operand);
                    break;
                case ExpressionType.Not:
                    Out((node.Type == typeof (bool) || node.Type == typeof (bool?)) ? '!' : '~');
                    Visit(node.Operand);
                    break;
                case ExpressionType.Quote:
                    Visit(node.Operand);
                    break;
                case ExpressionType.PostIncrementAssign:
                    Visit(node.Operand);
                    Out("++");
                    break;
                case ExpressionType.PostDecrementAssign:
                    Visit(node.Operand);
                    Out("--");
                    break;
                default:
                    Out(node.NodeType.ToString());
                    Out('(');
                    Visit(node.Operand);
                    Out(')');
                    break;
            }
            return node;
        }


        private static string GetConstantSuffix(Type type)
        {
            if (type == typeof (uint))
                return "U";
            if (type == typeof (long))
                return "L";
            if (type == typeof (ulong))
                return "UL";
            if (type == typeof (double))
                return "D";
            if (type == typeof (float))
                return "F";
            if (type == typeof (Decimal))
                return "M";
            return "";
        }


        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value != null)
            {
                var s = node.Value.ToString();
                if (node.Value is string)
                {
                    Out('"');
                    Out(s);
                    Out('"');
                }
                else if (node.Value is char)
                {
                    Out('\'');
                    Out(s);
                    Out('\'');
                }
                else if (s == node.Value.GetType().ToString())
                {
                    Out("const(");
                    Out(s);
                    Out(')');
                }
                else
                {
                    Out(s);
                    Out(GetConstantSuffix(node.Type));
                }
            }
            else
                Out("null");
            return node;
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            Out("default(");
            OutType(node.Type);
            Out(')');
            return node;
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            Out('(');
            Visit(node.Expression);
            switch (node.NodeType)
            {
                case ExpressionType.TypeIs:
                    Out(" Is ");
                    break;
                case ExpressionType.TypeEqual:
                    Out(" TypeEqual ");
                    break;
            }
            OutType(node.TypeOperand);
            Out(')');
            return node;
        }

        private int GetParameterId(ParameterExpression node)
        {
            return _paramIds.GetOrAdd(node, n => _paramIds.Count);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Out(string.IsNullOrEmpty(node.Name) ? "p_" + GetParameterId(node) : node.Name);
            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            Out("new ");
            OutType(node.Type);
            Out('(');
            var members = node.Members;
            for (var i = 0; i < node.Arguments.Count; ++i)
            {
                if (i > 0)
                    Out(", ");
                // ReSharper disable ConditionIsAlwaysTrueOrFalse
                if (members != null)
                    // ReSharper restore ConditionIsAlwaysTrueOrFalse
                {
                    Out(members[i].Name);
                    Out(" = ");
                }
                Visit(node.Arguments[i]);
            }
            Out(')');
            return node;
        }

        private void VisitExpressions(IEnumerable<Expression> expressions, string seperator = ", ", bool newLine = false)
        {
            if (expressions == null)
                return;
            var first = true;
            foreach (var obj in expressions)
            {
                if (first)
                    first = false;
                else
                    Out(seperator);
                if (newLine)
                    NewLine();
                Visit(obj);
            }
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            if (node.Object != null)
                Visit(node.Object);
            else
                OutType(node.Indexer.DeclaringType);
            if (node.Indexer != null)
            {
                Out(".");
                Out(node.Indexer.Name);
            }
            Out('[');
            VisitExpressions(node.Arguments);
            Out(']');
            return node;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            BeginBlock(false);
            foreach (var parameterExpression in node.Variables)
            {
                NewLine();
                OutType(parameterExpression.Type);
                Out(' ');
                VisitParameter(parameterExpression);
                Out(';');
            }
            VisitExpressions(node.Expressions, ";", true);
            Out(';');
            EndBlock();
            return node;
        }


        private int GetLabelId(LabelTarget node)
        {
            return _labelIds.GetOrAdd(node, n => _paramIds.Count);
        }

        private void OutLabel(LabelTarget target)
        {
            Out(string.IsNullOrEmpty(target.Name) ? ("l_" + GetLabelId(target)) : target.Name);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            Out(node.Kind.ToString().ToLower());
            Out(' ');
            OutLabel(node.Target);
            if (node.Value == null)
                return node;
            Out('(');
            Visit(node.Value);
            Out(')');
            return node;
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            //Out("{ ... } ");
            OutLabel(node.Target);
            Out(":");
            return node;
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            OutLabel(node);
            return node;
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            Out("catch (");
            OutType(node.Test);
            if (node.Variable != null)
                Out(node.Variable.Name);
            Out(')');
            BeginBlock();
            Visit(node.Body);
            EndBlock();
            return node;
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            Out("IF(");
            Visit(node.Test);
            Out(", ");
            Visit(node.IfTrue);
            Out(", ");
            Visit(node.IfFalse);
            Out(")");
            return node;
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            if (node.Arguments.Count == 1)
                Visit(node.Arguments[0]);
            else
            {
                Out('{');
                for (var i = 0; i < node.Arguments.Count; i++)
                {
                    if (i > 0)
                        Out(',');
                    Visit(node.Arguments[i]);
                }
                Out('}');
            }
            return node;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            Out(node.Member.Name);
            Out(" = ");
            Visit(node.Expression);
            return node;
        }

        private void OutMember(Expression instance, MemberInfo member)
        {
            if (instance != null)
            {
                Visit(instance);
                Out("." + member.Name);
            }
            else
            {
                // ReSharper disable PossibleNullReferenceException
                OutType(member.DeclaringType);
                Out('.');
                Out(member.Name);
                // ReSharper restore PossibleNullReferenceException
            }
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            OutMember(node.Expression, node.Member);
            return node;
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            Visit(node.NewExpression);
            BeginBlock();
            for (var i = 0; i < node.Bindings.Count; ++i)
            {
                if (i > 0)
                    Out(", ");
                NewLine();
                var binding = node.Bindings[i];
                VisitMemberBinding(binding);
            }
            EndBlock();
            return node;
        }

        private void EndBlock()
        {
            Indent(-1);
            NewLine();
            Out("}");
        }

        private void BeginBlock(bool newLine = true)
        {
            Out("{");
            if (newLine)
                NewLine();
            Indent(1);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            Visit(node.Expression);
            Out('(');
            VisitExpressions(node.Arguments);
            Out(')');
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Object == null)
                OutType(node.Method.DeclaringType);
            else
                Visit(node.Object);
            Out('.');
            Out(node.Method.Name);
            Out('(');
            VisitExpressions(node.Arguments);
            Out(')');
            return node;
        }

        protected override Expression VisitTry(TryExpression node)
        {
            Out("try");
            BeginBlock();
            Visit(node.Body);
            EndBlock();
            foreach (var handler in node.Handlers)
            {
                VisitCatchBlock(handler);
            }
            if (node.Fault != null)
            {
                Out("catch");
                BeginBlock();
                Visit(node.Fault);
                EndBlock();
            }
            // ReSharper disable once InvertIf
            if (node.Finally != null)
            {
                Out("finally");
                BeginBlock();
                Visit(node.Finally);
                EndBlock();
            }
            return node;
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            Out("/*Debug info*/");
            return node;
        }

#if NET
        protected override Expression VisitDynamic(DynamicExpression node)
        {
            throw new NotImplementedException();
        }
#endif

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            foreach (var expression in node.TestValues)
            {
                NewLine();
                Out("case ");
                Visit(expression);
                Out(": ");
            }
            Indent(1);
            NewLine();
            Visit(node.Body);
            Out(';');
            Indent(-1);
            return node;
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            Out("switch (");
            Visit(node.SwitchValue);
            Out(')');
            BeginBlock(false);
            foreach (var switchCase in node.Cases)
            {
                VisitSwitchCase(switchCase);
            }
            if (node.DefaultBody != null)
            {
                NewLine();
                Out("default:");
                Indent(1);
                NewLine();
                Visit(node.DefaultBody);
                Indent(-1);
            }
            EndBlock();
            return node;
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            Out("var (");
            VisitExpressions(node.Variables);
            Out(')');
            return node;
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.NewArrayInit:
                    Out("new [] ");
                    Out('{');
                    VisitExpressions(node.Expressions);
                    Out('}');
                    break;
                case ExpressionType.NewArrayBounds:
                    Out("new ");
                    OutType(node.Type.GetElementType());
                    Out('[');
                    VisitExpressions(node.Expressions);
                    Out(']');
                    break;
            }
            return node;
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            Visit(node.NewExpression);
            Out('{');
            for (var i = 0; i < node.Initializers.Count; ++i)
            {
                if (i > 0)
                    Out(", ");
                VisitElementInit(node.Initializers[i]);
            }
            Out('}');
            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Out('(');
            for (var i = 0; i < node.Parameters.Count; i++)
            {
                if (i > 0)
                    Out(", ");
                OutType(node.Parameters[i].Type);
                Out(' ');
                VisitParameter(node.Parameters[i]);
            }
            Out(')');
            Out(" => ");
            Visit(node.Body);
            return node;
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            Out("Loop ");
            if (node.ContinueLabel != null)
                OutLabel(node.ContinueLabel);
            BeginBlock();
            Visit(node.Body);
            EndBlock();
            if (node.BreakLabel != null)
                OutLabel(node.BreakLabel);
            return node;
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            Out(node.Member.Name);
            Out(" = {");
            for (var i = 0; i < node.Initializers.Count; ++i)
            {
                if (i > 0)
                    Out(", ");
                VisitElementInit(node.Initializers[i]);
            }
            Out('}');
            return node;
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            Out(node.Member.Name);
            Out(" = ");
            BeginBlock(false);
            for (var i = 0; i < node.Bindings.Count; ++i)
            {
                if (i > 0)
                    Out(", ");
                NewLine();
                VisitMemberBinding(node.Bindings[i]);
            }
            EndBlock();
            return node;
        }

        protected override Expression VisitExtension(Expression node)
        {
            if (node.GetType().Method("ToString", TypeHelper<Type>.EmptyArray).DeclaringType != typeof (Expression))
            {
                Out(node.ToString());
                return node;
            }
            Out('[');
            Out(node.NodeType == ExpressionType.Extension ? node.GetType().FullName : node.NodeType.ToString());
            Out(']');
            return node;
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}