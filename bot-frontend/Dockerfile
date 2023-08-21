FROM python:3.11
WORKDIR /bot-frontend
COPY ./ ./
RUN pip install poetry
RUN poetry config virtualenvs.create false && \
    poetry install --no-interaction --no-ansi --no-root
CMD ["poetry", "run", "python", "app.py"]