from flask import Flask, render_template, request, redirect, url_for
from keras.models import load_model
import numpy as np

import tensorflow_datasets as tfds
import numpy as np

model_pipeline = load_model("reviewCheck_V1.h5")

result = ""

app = Flask(__name__, template_folder="pages")

def is_deceptive(query):
    tokenizer_tfds=tfds.deprecated.text.SubwordTextEncoder.build_from_corpus([query],1000,max_subword_length=5)
    for i,sent in enumerate(temp_sentences):
        temp_sentences[i]=tokenizer_tfds.encode(sent)
    sequence_added=pad_sequences(temp_sentences,maxlen=max_length,padding =padding_type,truncating=trunc_type)
    test_seq=sequence_added
    return model_pipeline.predict(test_seq[0:1])[0][0]

@app.route('/')
def home():
    return render_template('forms/Home.html')


@app.route('/', methods=['POST', 'GET'])
def get_data():
    if request.method == 'POST':
        query = request.form['search']
        result = is_deceptive(query)
        return render_template('forms/Home.html', res=result)


if __name__ == '__main__':
    app.run(debug=True)
