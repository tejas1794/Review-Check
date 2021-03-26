from flask import Flask, render_template, request, redirect, url_for
from keras.models import load_model
from keras.preprocessing.sequence import pad_sequences
import tensorflow_datasets as tfds
import pandas as pd
from joblib import load
from sklearn.feature_extraction.text import TfidfVectorizer
from sklearn.feature_extraction.text import ENGLISH_STOP_WORDS

model_pipeline = load("model.joblib")
#import numpy as np

#model_pipeline = load_model("reviewCheck_V1.h5")

result = ""

app = Flask(__name__, template_folder="pages")


def is_deceptive(query):
    if str(model_pipeline.predict([query])[0]) == "0":
        return "Real"
    return "Deceptive"
    # # Amazon Data
    # input_file = "amazon_reviews.txt"
    # amazon = pd.read_csv(input_file, delimiter='\t')
    # amazon['source'] = 'amazon'
    #
    # # combine all data sets
    # data = pd.DataFrame()
    # data = pd.concat([amazon])
    # temp_sentences = data['REVIEW_TEXT'].tolist()
    # tokenizer_tfds = tfds.deprecated.text.SubwordTextEncoder.build_from_corpus(temp_sentences, 1000, max_subword_length=5)
    # query = tokenizer_tfds.encode(query)
    # sequence_added = pad_sequences(query, maxlen=100, padding='post', truncating='post')
    # test_seq = sequence_added
    # return model_pipeline.predict(test_seq[0:1])[0][0]


@app.route('/', methods=['GET','POST'])
def get_data():
    if request.method == 'POST':
        query = request.data    
        result = is_deceptive(query)
    return result #render_template('forms/Home.html', res=result)


if __name__ == '__main__':
    app.run(debug=True)
