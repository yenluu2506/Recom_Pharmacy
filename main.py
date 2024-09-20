import pyodbc
import pandas as pd
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.metrics.pairwise import cosine_similarity
from flask import Flask, jsonify, request

app = Flask(__name__)

server = 'YENLUUIT01\\SQLEXPRESS'
database = 'RecomPharmacy'
username = ''
password = ''

connection_String = f'DRIVER={{SQL Server}};SERVER={server};DATABASE={database};UID={username};password={password}'

try:
    conn = pyodbc.connect(connection_String)
    query = 'Select * from THUOC where TRANGTHAI=1'
    df_thuoc = pd.read_sql(query, conn)
    print(df_thuoc.head())

except pyodbc.Error as e:
    print(f'Error:{e}')
finally:
    if conn:
        conn.close()

features = ['CONGDUNG', 'GIABAN', 'MOTA']

def combineFeatures(row):
    return str(row['GIABAN']) + " " + str(row['CONGDUNG']) + " " + str(row['MOTA'])

df_thuoc['combinedFeatures'] = df_thuoc.apply(combineFeatures,axis=1)

print(df_thuoc['combinedFeatures'].head())

tf = TfidfVectorizer()
tfMatrix = tf.fit_transform(df_thuoc['combinedFeatures'])

similar = cosine_similarity(tfMatrix)

number = 4
@app.route('/api', methods=['GET'])
def get_data():
    ket_qua = []
    productid = request.args.get('id')
    productid = int (productid)

    if productid not in df_thuoc['ID'].values:
        return jsonify({'Loi':'ID khong hop le'})
    indexproduct = df_thuoc[df_thuoc['ID']==productid].index[0]

    similarProduct = list(enumerate(similar[indexproduct]))

    print(similarProduct)

    sortedSimilarProduct = sorted(similarProduct, key=lambda x:x[1], reverse=True)

    def lay_ten(index):
        return (df_thuoc[df_thuoc.index == index]['TENTHUOC'].values[0])


    for i in range(1, number + 1):
        print(lay_ten(sortedSimilarProduct[i][0]))
        ket_qua.append(lay_ten(sortedSimilarProduct[i][0]))

    data = {'San pham goi y': ket_qua}
    return jsonify(data)

if __name__ == '__main__':
    app.run(port=5555)