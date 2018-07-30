var requestURL = 'https://mdn.github.io/learning-area/javascript/oojs/json/superheroes.json';

var request = new XMLHttpRequest();
request.open('GET', requestURL);

request.responseType = 'json';
request.send();

request.onload = function() {
    var superHeroes = request.response;
    populateHeader(superHeroes);
    showHeroes(superHeroes);
}

function populateHeader(jsonData) {
    let content = document.createElement("div");
    let name = document.createElement("h1");
    let line = document.createElement("div");
    let home = document.createElement("p");
    let founded = document.createElement("p");

    content = content.appendChild(document.createElement("p"));
    name.innerHTML = jsonData['squadName'];
    home.innerHTML = jsonData['homeTown'];
    founded.innerHTML = jsonData['formed'];

    home.className = "home";
    founded.className = "founded";

    content.appendChild(name);
    line.appendChild(home);
    line.appendChild(founded);
    content.appendChild(line);

    header.appendChild(content);
}

function showHeroes(jsonData) {
    let members = jsonData['members']
    for (var i = 0; i < members.length; i++) {
        let template = document.createElement("article");
        let template_name = document.createElement("h2");
        let template_age = document.createElement("p");
        let template_secretIdentity = document.createElement("p");
        let power_header = document.createElement("p");
        let template_powers = document.createElement("ul");

        template_name.innerText = members[i]['name'];
        template_age.innerText = "Age: " + members[i]['age'];
        template_secretIdentity.innerText = "Secret identity: " + members[i]['secretIdentity'];

        power_header.innerText = "Superpowers:";

        for (var j = 0; j < members[i]['powers'].length; j++) {
            let listItem = document.createElement("li");
            listItem.innerText = members[i]['powers'][j];
            template_powers.appendChild(listItem);
        }

        template.appendChild(template_name);
        template.appendChild(template_age);
        template.appendChild(template_secretIdentity);
        template.appendChild(power_header);
        template.appendChild(template_powers);
        section.appendChild(template);
    }
}