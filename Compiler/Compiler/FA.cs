using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Substring
    {
        string str;
        int idx;

        public Substring(string str_, int idx_)
        {
            str = str_;
            idx = idx_;
        }

        public string GetStr()
        {
            return str;
        }

        public int GetIdx()
        {
            return idx;
        }
    }
    
    class FA
    {
        public List<Substring> Find(string str)
        {
            List<Substring> list = new List<Substring>();
            int startPosition = 0;

            while (startPosition != str.Length)
            {
                int state = 0;
                int index = startPosition;
                string sub = "";

                while (index != str.Length)
                {
                    char c = str[index];

                    if (c != '0' && c != '1')
                    {
                        break;
                    }

                    switch (state)
                    {
                        case 0:
                            if (c == '0')
                            {
                                state = 1;
                                sub += c;
                            }
                            else if (c == '1')
                            {
                                state = 0;
                                sub += c;
                            }
                            break;
                        case 1:
                            if (c == '0')
                            {
                                state = 2;
                                sub += c;
                            }
                            else if (c == '1')
                            {
                                state = 3;
                                sub += c;
                            }
                            break;
                        case 2:
                            if (c == '0')
                            {
                                state = 4;
                                sub += c;
                                list.Add(new Substring(sub, startPosition));
                            }
                            else if (c == '1')
                            {
                                state = 5;
                                sub += c;
                                list.Add(new Substring(sub, startPosition));
                            }
                            break;
                        case 3:
                            if (c == '0')
                            {
                                state = 6;
                                sub += c;
                                list.Add(new Substring(sub, startPosition));
                            }
                            else if (c == '1')
                            {
                                state = 7;
                                sub += c;
                                list.Add(new Substring(sub, startPosition));
                            }
                            break;
                        case 4:
                            if (c == '0')
                            {
                                state = 4;
                                sub += c;
                                list.Add(new Substring(sub, startPosition));
                            }
                            else if (c == '1')
                            {
                                state = 5;
                                sub += c;
                                list.Add(new Substring(sub, startPosition));
                            }
                            break;
                        case 5:
                            if (c == '0')
                            {
                                state = 6;
                                sub += c;
                                list.Add(new Substring(sub, startPosition));
                            }
                            else if (c == '1')
                            {
                                state = 7;
                                sub += c;
                                list.Add(new Substring(sub, startPosition));
                            }
                            break;
                        case 6:
                            if (c == '0')
                            {
                                state = 2;
                                sub += c;
                            }
                            else if (c == '1')
                            {
                                state = 3;
                                sub += c;
                            }
                            break;
                        case 7:
                            if (c == '0')
                            {
                                state = 1;
                                sub += c;
                            }
                            else if (c == '1')
                            {
                                state = 0;
                                sub += c;
                            }
                            break;
                    }
                    index++;
                }
                startPosition++;
            }
            
            return list;
        }
    }


}
