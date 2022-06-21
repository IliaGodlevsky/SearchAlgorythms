﻿using System;
using System.Linq;

namespace ConsoleVersion.Model
{
    internal readonly struct Answer : IComparable, IComparable<Answer>, IEquatable<Answer>
    {
        private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;
        
        public static readonly Answer Yes = new Answer(1, nameof(Yes));
        public static readonly Answer No = new Answer(0, nameof(No));
        private static readonly Answer Default = new Answer(-1, string.Empty);
        private static readonly Answer[] Answers = new[] { Yes, No };

        private readonly int value;
        private readonly string display;

        private Answer(int value, string display)
        {
            this.display = display;
            this.value = value;
        }

        public int CompareTo(Answer other)
        {
            return value.CompareTo(other.value);
        }

        public int CompareTo(object obj)
        {
            return obj is Answer answer ? this.CompareTo(answer) : throw new ArgumentException("Wrong argument", nameof(obj));
        }

        public bool Equals(Answer other)
        {
            return other.value == value && display.Equals(other.display, IgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is Answer answer ? this.Equals(answer) : false;
        }

        public override string ToString()
        {
            return display;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(value, display);
        }

        public static bool TryParse(string input, out Answer result)
        {
            if (int.TryParse(input, out int value))
            {
                result = FirstOrDefault(Answers, answer => answer.value.Equals(value));
                return !result.Equals(Default);
            }
            result = FirstOrDefault(Answers, answer => answer.display.Equals(input, IgnoreCase));
            return !result.Equals(Default);
        }

        public static implicit operator bool(Answer answer)
        {
            return answer.value == 1;
        }

        public static implicit operator int(Answer answer)
        {
            return answer.value;
        }

        private static Answer FirstOrDefault(Answer[] answers, Func<Answer, bool> predicate)
        {
            return answers.Any(predicate) ? answers.FirstOrDefault(predicate) : Answer.Default;
        }
    }
}