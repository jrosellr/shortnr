FROM node:20.10.0-bullseye as base

WORKDIR /app

COPY package.json .
COPY .npmrc .

RUN npm install

COPY . .
EXPOSE 4200

CMD ["npm", "run", "start:container"]

