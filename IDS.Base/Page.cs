namespace IDS.Base
{
    public class Page<T>
    {
        public int pageSize { set; get; }
        public int? total { set; get; }//总条数
        public int current { set; get; }
        //{
        //    set { 
        //         current = value;
        //    } 
        //    get {
        //        return current - 1;
        //    }
        //}   //当前第几页
        public T? requestData { set; get; }
        public List<T>? data { set; get; }
        public Page(int? total,List<T> data,int pageSize,int currentPage) {
            this.pageSize = pageSize;
            this.total = total;
            this.current = currentPage;
            this.data = data;
        }
        public Page() { }

}

 
}
