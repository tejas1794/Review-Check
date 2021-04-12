from flask import Flask, render_template, request, redirect, url_for
from joblib import load

model_pipeline = load("model.joblib")

app = Flask(__name__, template_folder="pages")


def is_deceptive(query):
    if str(model_pipeline.predict([query])[0]) == "0":
        return "Real"
    return "Deceptive"


@app.route('/', methods=['GET', 'POST'])
def get_data():
    result = ""
    if request.method == 'POST':
        query = request.data
        result = is_deceptive(query)
    return result


if __name__ == '__main__':
    app.run(debug=True)
