using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace osu_chat
{
    public class UserCollection : IEnumerable<string>
    {
        
        List<string> generic;

        public UserCollection()
        {
            generic = new List<string>();
        }

        public UserCollection(string[] source)
        {
            if (source != null)
                generic = new List<string>(source);
            else
                generic = new List<string>();
        }
                
        public virtual string this[int index]
        {
            get
            {
                return generic[index];
            }
            set
            {
                generic[index] = value;
            }
        }
        
        public virtual void Add(string Username)
        {
            if (generic.IndexOf(Username) == -1)
            {
                generic.Add(Username);
                UserAdded?.Invoke(null, Username);
            }
        }

        public virtual void AddRange(string[] Usernames)
        {
            for (int i = 0; i < Usernames.Length; i++)
                if (!this.Exists(Usernames[i]))
                {
                    generic.Add(Usernames[i]);
                    UserAdded?.Invoke(null, Usernames[i]);
                }
        }

        public virtual void Remove(string Username)
        {
            if (generic.IndexOf(Username) != -1)
            {
                generic.Remove(Username);
                UserRemoved?.Invoke(null, Username);
            }
        }

        public virtual void Insert(int index, string Username)
        {
            if (generic.IndexOf(Username) == -1)
            {
                generic.Insert(index, Username);
                UserAdded?.Invoke(null, Username);
            }
        }

        public virtual bool Exists(string Username)
        {
            return generic.IndexOf(Username) != -1;
        }

        public virtual int IndexOf(string  Username)
        {
            return generic.IndexOf(Username);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return generic.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return generic.GetEnumerator();
        }

        public string[] ToArray()
        {
            return generic.ToArray<string>();
        }

        public event EventHandler<string> UserAdded;
        public event EventHandler<string> UserRemoved;


    }
}
