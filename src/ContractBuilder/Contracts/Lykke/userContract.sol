pragma solidity ^0.4.1;

contract QNTB {
    function transfer(address _to, uint64 _amount) public returns (bool);
}

contract UserContract {
    address _owner;
    address _asset;

    modifier onlyowner { if (msg.sender == _owner) _; }

    function UserContract(address asset) {
        _owner = msg.sender;
        _asset = asset;
    }

    function() payable {
        throw;
    }

    function transferMoney(address recepient, uint64 value) onlyowner {
        var bank = QNTB(_asset);
        if (!bank.transfer(recepient, value))
            throw;
    }
}