import  xdrlib ,sys
import xlrd
import os
import logging


def open_excel(file= 'file.xls'):
    try:
        data = xlrd.open_workbook(file)
        return data
    except Exception,e:
        print str(e)
def excel_table_byindex(file= 'D:/mytestforpython.xlsx',colnameindex=0,by_index=0):
    data = open_excel(file)
    table = data.sheet_by_index(by_index)
    nrows = table.nrows 
    ncols = table.ncols 
    colnames =  table.row_values(colnameindex)

    filename=os.path.basename(file)[:-5]
    filepath=os.path.dirname(file)



    for rownum in range(2,nrows):
         row = table.row_values(rownum)
         if row:
             json="name:"
             for i in range(len(colnames)):
                json+= str( row[i])+","
             json+="\n"
             
             file_object = open("D:/mytestforpython.json", 'a')
             file_object.write(json)
             file_object.close()
    return 
def main():
    excel_table_byindex()

if __name__=="__main__":
    main()