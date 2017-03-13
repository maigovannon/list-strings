using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumpStrings {
   class DumpStrings {
      public DumpStrings (string path) {
         mText = "  " + File.ReadAllText (path).Replace ("\r\n", "\n") + "  ";
      }

      /// <summary>This runs the string parsing algorithm and dumps the output to the console</summary>
      public void Dump () {
         mIdx = 2;
         while (mIdx < mText.Length) {
            PrintNextStr ();
            mIdx++;
         }
      }

      void PrintWS (int c) {
         var def = Console.ForegroundColor;
         Console.ForegroundColor = ConsoleColor.Cyan;
         while (c-- > 0) Console.Write ('_');
         Console.ForegroundColor = def;
      }

      void PrintNextStr () {
         ToNextQuote (true); if (mIdx >= mText.Length) return;
         var start = ++mIdx;
         ToNextQuote (false); if (mIdx >= mText.Length) return;

         Console.Write (++count + ", ");
         var str = mText.Substring (start, mIdx - start);

         var cnt = str.TakeWhile (c => c == ' ').Count ();
         PrintWS (cnt);
         var trim = str.Trim (); cnt += trim.Length;
         Console.Write (trim);
         PrintWS (str.Length - cnt);
         Console.WriteLine ();
      }


      void ToNextQuote (bool open) {
         while (mIdx < mText.Length) {
            if (IsValidQuote (open)) return;
            if (open) SkipComment ();
            mIdx++;
         }
      }

      bool IsValidQuote (bool open) {
         if (mText[mIdx] != '"') return false;
         char c = '"', d = '"', e = '\"';
         if (open) return mText[mIdx - 1] != '\'' && mText[mIdx - 1] != '\\';
         // Close quote "bhial"*/
         int i = mIdx - 1;
         bool escaped = false;/*asdfdsa
         asdfsad
         "asdfsda"
         */
         while (i < mText.Length && mText[i] == '\\') {
            escaped = !escaped;
            i--;
         }
         return !escaped;
      }

      void SkipComment () {
         string str = mText.Substring (mIdx - 1, 2);
         if (str == MultiComment) {
            GetCommentEnd (Newline);
         } else if (str == SLCommentStart) {
            GetCommentEnd (SLCommentEnd);
         } else return;
      }

      void GetCommentEnd (string delim) {
         while (mIdx < mText.Length && mText.Substring (mIdx++ - delim.Length, delim.Length) != delim) ;
      }

      const string Newline = "\n";
      const string MultiComment = "//";
      const string SLCommentStart = "/*";
      const string SLCommentEnd = "*/";

      int mIdx;
      string mText;
      long count = 0;
   }

   class Program {
      static void Main (string[] args) {
         if (args.Length < 0) Console.WriteLine ("Enter a valid filename");
         var path = args[0];
         if (string.IsNullOrEmpty (path) || !File.Exists (path)) {
            Log (ErrMsg, path);
            return;
         }
         var parser = new DumpStrings (path);
         parser.Dump ();
      }

      static void Log (string msg, params object[] args) => Console.WriteLine (string.Format (msg, args));

      static readonly string ErrMsg = "The file \"{0}\" is not valid or does not exist";
   }
}
