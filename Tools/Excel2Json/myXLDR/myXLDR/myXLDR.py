# -*- coding: utf-8 -*- 
import  xdrlib ,sys
import xlrd
import os
def open_excel(file= 'file.xls'):
    try:
        data = xlrd.open_workbook(file)
        return data
    except Exception,e:
        print str(e)
#根据索引获取Excel表格中的数据   参数:file：Excel文件路径     colnameindex：表头列名所在行的索引  ，by_index：表的索引
def excel_table_byindex(file= 'D:/mytestforpython.xlsx',colnameindex=0,by_index=0):
    data = open_excel(file)
    table = data.sheet_by_index(by_index)
    nrows = table.nrows #行数
    ncols = table.ncols #列数
    colnames =  table.row_values(colnameindex) #某一行数据 

    filename=os.path.basename(file)[:-5]
    filepath=os.path.dirname(file)

    if os.path.exists("./out"+filepath) ==False : 
           os.makedirs("./out"+filepath)
    if os.path.exists('./out'+filepath+"/"+filename+".json")==True:
           os.remove('./out'+filepath+"/"+filename+".json")

    for rownum in range(1,nrows):

         row = table.row_values(rownum)
         if row:
             json=""
             for i in range(len(colnames)):
                json+= colnames[i] +":"+str( row[i])+","
             json+="\n"
             
             file_object = open('./out'+filepath+"/"+filename+".json", 'a')
             file_object.write(json)
             file_object.close()
    return 

def dirlist(path,allfile):  
    filelist =  os.listdir(path)  
  
    for filename in filelist:  
        filepath = os.path.join(path, filename)  
        if os.path.isdir(filepath):  
            dirlist(filepath, allfile) 
            
        else:
            if filepath.find(".xlsx") != -1:
                excel_table_byindex(filepath)
    return allfile
    
def main():
    dirlist(".",[])



if __name__=="__main__":
    main()