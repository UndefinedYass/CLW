using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.ComponentModel;



namespace System
{
    public interface IDevice : CLW.Model. IDeviceBase
    {
        void Plug(IDevice parrentDevice);
    }
}

namespace CLW.Model
{
    public class ET
    {

        public delegate string StringPropertyExtractor(string rawItem);
        public delegate string Transformer( params string[] p);

        public static StringPropertyExtractor BuildPropertyExtractorExpression(ListWatcherCPE ParentLWCPE=null, TargetPropertyCPE propCPE=null )
        {
            Func<int, int>  f1= ((i) => i + 2);
            Func<int, int> f2 = ((i) => i + 2);
            Func<int, int> comb = (i) => f2(f1(i));
            var i_param = Expression.Parameter(typeof(int));
            Expression add1_body = Expression.Add(Expression.Constant(1), i_param);
            Expression<Func<int,int>> add1_lm = Expression.Lambda<Func<int,int>>(add1_body, i_param);

            var i2_param = Expression.Parameter(typeof(int));
        
            //transformation chains are bult bottom-up, starting from the targetPropCPE
               
            Expression add2_body = Expression.Add(Expression.Constant(2), i2_param);
            Expression<Func<int, int>> add2_lm = Expression.Lambda<Func<int, int>>(add2_body, i2_param);
            var global_i = Expression.Parameter(typeof(int));
            Expression call1 = Expression.Invoke(add1_lm, Expression.Invoke(add2_lm, global_i));
            var combined_ex = Expression.Lambda<Func<int, int>>(call1, global_i);
            MainWindow.ShowMessage(combined_ex.Compile()(7).ToString());
            return new StringPropertyExtractor((r) => r);
        }

        
        public static Expression<Transformer> GoF(Expression<Transformer> G, Expression<Transformer> F)
        {
            var global_i = Expression.Parameter(typeof(string));
            Expression call1 = Expression.Invoke(G, Expression.Invoke(F, global_i));
            return Expression.Lambda<Transformer>(call1, global_i);
        }
    }





    public delegate void OutputChangedEventHandler();

    public interface IDeviceBase
    {
       
        void DoWork();
        bool IsPluged { get; }
        bool IsRoot { get; }
        bool IsTransparent { get; }
        event OutputChangedEventHandler OutputChanged;
    }

    
     public interface IDevice: IDeviceBase
    {
        void Plug(IDevice parrentDevice);
    }

    public interface IRootDevice: IDeviceBase
    {
        void Push(string rawInput);
    }




    public abstract class Device : IDevice
    {

        public Device(IDevice parent)
        {
            Plug(parent);
        }
        public bool IsPluged
        {
            get;
            
            internal set;
        }

        public bool IsRoot
        {
            get
            {
                return false;
            }
        }

        public bool IsTransparent
        {
            get; set;
        }

        public event OutputChangedEventHandler OutputChanged;

        public abstract void DoWork();

        public void Plug(IDevice parrentDevice)
        {
            IsPluged = true;
        }

     
    }





}
