from BaseHTTPServer import BaseHTTPRequestHandler, HTTPServer
import SocketServer
import json

USER_DB = './user_db'
init_data = {"Enemy": {"Hellephant": [], "Zombunny": [], "ZomBear": []}, "Item": {"Cake": [], "HealthPack": [], "Apple": [], "Pie": []}, "state": 1, "health": 100, "exp": 0, "Position": {"y": -1.484156e-05, "x": -3.581895, "z": -7.566027}}

class S(BaseHTTPRequestHandler):
    def _set_headers(self):
        self.send_header('Content-type', 'text/html')
        self.end_headers()

    def do_HEAD(self):
        self._set_headers()

    def do_PUT(self):
        # self._set_headers()
        length = int(self.headers['Content-Length'])
        msg = self.rfile.read(length)
        print(msg)

        msg_dict = json.loads(msg)
        username = msg_dict["value"]
        db_dict = load_dict(USER_DB)
        if(msg_dict["type"] == "login"):
            if username in db_dict:
                self.send_response(200)
            else:
                self.send_response(404)
        elif(msg_dict["type"] == "register"):
            if username in db_dict:
                self.send_response(500)
            else:
                db_dict[username] = init_data
                write_dict(USER_DB, db_dict)
                self.send_response(201)

        self._set_headers()
        

def load_dict(file_path):
    try:
        with open(file_path) as f:
            load_dict = json.load(f)
        return load_dict
    except Exception as e:
        print(file_path + ' load fail: ' + str(e))
        return {}

def write_dict(file_path, data_dict):
    try:
        with open(file_path, 'w') as f:
            json.dump(data_dict, f)
    except Exception as e:
        print(file_path + ' write fail: ' + str(e))
        return {}
        
def run(server_class=HTTPServer, handler_class=S, port=5567):
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    print 'Starting httpd...'
    httpd.serve_forever()

if __name__ == "__main__":
    from sys import argv

    if len(argv) == 2:
        run(port=int(argv[1]))
    else:
        run()