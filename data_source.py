import requests
import json
import os


def get_nuke_code_rougetrader():
    # 伪造header
    url = 'https://a.roguetrader.com/graphql'
    headers = {
        'origin': 'https://roguetrader.com',
        'accept-encoding': 'gzip, deflate, br',
        'user-agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.69 Safari/537.36 Edg/95.0.1020.44'
    }

    payload_data = {
        "operationName": "dashboard",
        "variables": "{}",
        "query": "query dashboard {\n  dashboard {\n    id\n    results\n    __typename\n  }\n}\n"
    }
    # 通过伪造的header请求数据包
    r = requests.post(url=url, data=payload_data, headers=headers, timeout=30)
    # 处理数据，获得密码
    data = json.loads(r.text)
    data_str = str(data)
    a, b, c, t = "'alpha': '", "'bravo': '", "'charlie': '", "'range'"
    a_code = data_str[data_str.find(a)+10:data_str.find(a)+18]
    b_code = data_str[data_str.find(b)+10:data_str.find(b)+18]
    c_code = data_str[data_str.find(c)+12:data_str.find(c)+20]
    time_start = data_str[data_str.find(t)+16:data_str.find(t)+21]
    time_end = data_str[data_str.find(t)+40:data_str.find(t)+45]

    codedict = {'A': a_code,
                'B': b_code,
                'C': c_code,
                "T_S": time_start,
                'T_E': time_end}

    # 你可能需要稍微更改一下路径
    os.chdir(
        '/root/go-cqhttp_linux_386/BOS-bot-v3/bos_bot_v3/plugins/nukacode/')
    with open("codelog.json", 'w') as f:
        json.dump(codedict, f, indent=4, ensure_ascii=False)


# get_nuke_code_rougetrader()
